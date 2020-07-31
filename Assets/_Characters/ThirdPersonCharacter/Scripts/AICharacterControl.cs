
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(NavMeshAgent))]
    //[RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        private NavMeshAgent Agent;           // the navmesh agent required for the path finding
        private AnimationController AnimController; // the character we are controlling
        private Camera cam;
        private Interactable ownInteractable;
        //TODO: create array of jobs with targets, (depending on intellegence lower priorities get forgotten)
        public Transform primaryTarget; // target to follow
        public Transform secondaryTarget;       // ba
        //public Interactable.ActionOption activeAction = Interactable.ActionOption.None; //TODO: array
        //public Vector3 activeDestination;
        public GameObject waypoint;
        private bool HasAgentReachedDestination()
        {
            // Check if we've reached the destination
            if (!Agent.pathPending)
            {
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Header("TestVariables")]

        public float targetDistance;
        [SerializeField] private bool _updateRotation;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] Vector3 desiredVelocity;
        [SerializeField] private float m_Turn;
        public bool followTarget;
        public BehaviourMode mode;
        public float desiredSocialization = 0;

        /// <summary>
        /// distance character tries to hold to enemy
        /// </summary>
        public float reach;
        /// <summary>
        /// distance to enemy at which Character prepares to fight
        /// </summary>
        public float range;
        /// <summary>
        /// Character will Attack enemy
        /// </summary>
        public bool IsInRange => Vector3.SqrMagnitude(primaryTarget.position - transform.position) < range * range;
        /// <summary>
        /// Character is able to land Hits
        /// </summary>
        public bool IsInReach => Vector3.SqrMagnitude(Agent.destination - transform.position) < reach * reach;
        /// <summary>
        /// Primary target can see you
        /// </summary>
        public bool IsVisible
        {
            get => Vector3.SqrMagnitude(primaryTarget.position - transform.position) < range * range;
            set => throw new System.NotImplementedException();
        }

        public bool IsTargetAlive()
        {
            if (primaryTarget.GetComponent<Interactable>())
            { 
                Interactable interactable = primaryTarget.GetComponent<Interactable>();
                if (interactable.statsControl.stats.hitpoints.current == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool isInRange;
        public bool isInReach;
        public bool isInSight;
        public bool isAlive;
        public bool hasStopped;
        [SerializeField] List<Interactable> watchlist;
        public Transform t_ManualTarget;

        public enum BehaviourMode
        {
            Idle,       // does stuff that he likes
            OnDuty,     // working, little interaction (Guarding...)
            Aggressive, // seeking and fighting enemies

        }

        //TODO: Waypoints - array mit targets + handler

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            Agent = GetComponentInChildren<NavMeshAgent>();
            AnimController = GetComponent<AnimationController>();
            cam = Camera.main;
            ownInteractable = GetComponentInParent<Interactable>();
            ownInteractable.OnActionExecute += OnActionExecute;

            Agent.updateRotation = true;
            Agent.updatePosition = true;
            
        }

        private void Update()
        {

            if (primaryTarget != null)
            {
                isInRange = IsInRange;
                isInReach = IsInReach;
                hasStopped = Agent.isStopped;
                isAlive = IsTargetAlive();
            }

            AdjustBehaviourMode();
            // TODO: Remove Test functions
            targetPosition = Agent.destination;
            targetDistance = Agent.remainingDistance - Agent.stoppingDistance;

            //TODO: possibility to continue task while direct control
            // Set Animation
            AnimController.SetMotion(Agent.velocity, false, false);
            Agent.nextPosition = transform.position;

            // stop rotation if target position < 1
            _updateRotation = Agent.updateRotation = UpdateRotation();
            Agent.updatePosition = false;
            Agent.updateRotation = true;

            // start action / job routine if any
            // ToDo: make coroutine or some shit
            if (HasAgentReachedDestination() && !ownInteractable.GetCurrentAction().destinationReached)
            {
                DestinationReached();
            }
        }

        private void FixedUpdate()
        {
            GetVisibleObjects();
            //TODO: Check TargetStatus to decide if too strong, weak, incapacitated...
        }

        // dont rotate while strafing
        private bool UpdateRotation()
        {
            Vector3 desiredVal = transform.InverseTransformVector(Agent.desiredVelocity);
            desiredVelocity = desiredVal;
            bool updaterot = Agent.isStopped || (Agent.remainingDistance > 0.501f && desiredVal.z > 0);

            return updaterot;
        }

        #region Action Event Functions 

        public void OnActionExecute(Interactable.ActionOption actionOption, Vector3 destination, Interactable target)
        {
            print("CharC " + name + " performs action " + actionOption + " destination " + destination + " target " + target);

            if (target)
            {
                // Click on other interactable
                if (target != ownInteractable)
                {
                    float stoppingDistance = 1f;
                    if (target.GetComponent<CapsuleCollider>())
                    {
                        
                        stoppingDistance = target.GetComponent<CapsuleCollider>().radius * target.transform.localScale.z + reach;
                    }
                    
                    Agent.stoppingDistance = stoppingDistance;
                    //primaryTarget = target.transform;
                    SetWaypoint(target.transform);
                }
            }
            else
            {
                print("Waypoint on floor");
                //primaryTarget = waypoint.transform;
                Agent.stoppingDistance = 0.2f;
                SetWaypoint(destination, true);
            }

            
            switch (actionOption)
            {
                case Interactable.ActionOption.EnterInteractable:
                    //move current target to secondary
                    //TODO: use waypoint for direct control
                    secondaryTarget = primaryTarget;
                    //primaryTarget = t_ManualTarget;
                    break;
                case Interactable.ActionOption.ExitInteractable:
                    primaryTarget = waypoint.transform; //secondaryTarget;
                    secondaryTarget = null;
                    break;
                case Interactable.ActionOption.Move:
                    //activeDestination = destination;
                    break;
                default:
                    
                    break;
            }
        }

        public void OnSelectedAction()
        {
            GetTargetFromMousePosition();
        }

        private void CancelCurrentAction()
        {
            ownInteractable.RemoveAction();
        }

        #endregion

        #region Navigation
        /// <summary>
        /// Tries to set Agent target to mouse position
        /// </summary>
        private Vector3 GetTargetFromMousePosition()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
            {
                return hit.point;
            }
            return Vector3.zero;
        }

        // if isControlled -> set secondary target
        // Base Function
        private bool SetWaypoint(Vector3 position)
        {
            if (NavMesh.SamplePosition(position, out NavMeshHit meshHit, 3.0f, NavMesh.AllAreas))
            {
                // remove waypoint from parent
                waypoint.transform.parent = null;
                waypoint.transform.position = meshHit.position;
                waypoint.SetActive(true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attaches waypoint to <i> objectTofollow </i> 
        /// </summary>
        /// <param name="objectTofollow">Interactable Object</param>
        /// <returns></returns>
        private void SetWaypoint(Transform objectTofollow)
        {
            waypoint.transform.parent = objectTofollow;
            waypoint.transform.localPosition = Vector3.zero;
            waypoint.SetActive(false);
        }

        /// <summary>
        /// Tries to set Agents' target to position
        /// </summary>
        private void SetWaypoint(Vector3 position, bool isActive)
        {
            if (!isActive)
            {
                waypoint.SetActive(false);
                return;
            }

            if (!SetWaypoint(position))
            {
                if (!SetWaypoint(GetTargetFromMousePosition()))
                {
                    waypoint.SetActive(false);
                    return;
                }
            }

            waypoint.SetActive(isActive);
        }
        /// <summary>
        /// Sets waypoint position equal to owner and deactivates it.
        /// </summary>
        private void ResetWaypoint()
        {
            // move waypoint to parent
            waypoint.transform.parent = transform;
            waypoint.transform.position = Vector3.zero;
            waypoint.SetActive(false);
        }

        private void DestinationReached()
        {
            if (!ownInteractable.GetCurrentAction().destinationReached)
            {
                ownInteractable.TargetReached();
            }
        }

        #endregion

        #region AIBehaviour

        private void AdjustBehaviourMode()
        {
            // TODO: Unterschied mouseTarget / calculatedTarget
            switch (mode)
            {
                case BehaviourMode.OnDuty:
                    // Direktsteuerung
                    Agent.stoppingDistance = 0.1f;
                    break;
                case BehaviourMode.Idle:
                    // immer target als destination setzen
                    if (primaryTarget != null && Agent.destination != primaryTarget.position)
                    {
                        Agent.SetDestination(primaryTarget.position);
                    }

                    break;
                case BehaviourMode.Aggressive:
                    // activate Battlemode
                    if (IsInRange)
                    {
                        AnimController.SetCombatMode(true);
                    }
                    if (IsInReach)
                    {
                        // Start fighting
                    }
                    if (primaryTarget != null && Agent.destination != primaryTarget.position)
                    {
                        // range länge in Richtung des characters
                        // TODO: reposition
                        Vector3 tempDestination = primaryTarget.position + (transform.position - primaryTarget.position) * reach;
                        print("Destination on Aggressive" + tempDestination);
                        Agent.SetDestination(primaryTarget.position + (transform.position - primaryTarget.position) * reach);
                    }

                    break;
                default:
                    break;
            }
        }
       
        private int GetTrustValueTo(int factionId)
        {
            return ownInteractable.GetTrustValueTo(factionId);
        }


        #endregion

        #region Collider triggers

        // SightTrigger Enter
        private void OnTriggerEnter(Collider other)
        {
            
            // Target erreicht
            if (other.gameObject == waypoint)
            {
                // Target nicht mehr folgen
                ResetWaypoint();
            }

            // Enter sight trigger -> Move to AIBehaviour
            if (other.GetComponent<Interactable>() != null && !other.isTrigger)
            {
                Interactable otherInteractable = other.GetComponent<Interactable>();
                watchlist.Add(otherInteractable);
                // TODO: Check faction -> if is enemy -> fight or flight
                int trustValue = GetTrustValueTo(otherInteractable.FactionId);
                if (trustValue < 20)
                {
                    print(name + " wants to fight " + otherInteractable.name);
                    //stop what you do 
                   
                    ownInteractable.CancelActions();
                    // Check if call interactable
                    ownInteractable.PerformAction(Interactable.ActionOption.Attack, otherInteractable);
                }
                else if (trustValue < 60)
                {
                    print(name + " is meh to " + otherInteractable.name);
                    // do nothing
                    //TODO: 51 - 60 Simply greet
                }
                else
                {
                    if (desiredSocialization < 5)
                    {
                        desiredSocialization += 1;
                        return;
                    }
                    desiredSocialization = 0;
                    
                    // Check if call interactable
                    Action talk = new Action(Vector3.zero, 0, Interactable.ActionOption.Talk, otherInteractable);
                    ownInteractable.InsertAction(talk);
                    Invoke("CancelCurrentAction", 10.0f);
                    print(name + " wants to talk to " + otherInteractable.name);
                    //TODO: Dont allways talk
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.GetComponent<Terrain>())
            {
                Debug.Log("collision with " + collision.gameObject.name + " enter");
                DestinationReached();
            }
            
        }

        // Exit Sight trigger
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Interactable>() != null)
            {
                
                Interactable otherInteractable = other.GetComponent<Interactable>();
                watchlist.Remove(otherInteractable);
            }
            
            
        }
        #endregion

        #region Senses
        // Invoke every 100 ms
        void GetVisibleObjects()
        {
            isInSight = false;
            foreach (var item in watchlist)
            {
                //TODO: remove if empty
                if (!item)
                {
                    watchlist.Remove(item);
                }
                if (IsObjectInSight(item.transform))
                {
                    // TODO: chooseTarget from visible enemies function
                    // TODO: 
                    isInSight = true;
                }
            }
        }

        // TODO: Testen: mit Physics.OverlapSphere besser?
        // Todo: gegen Ecken und zentrum des skinnedMeshRenderers prüfen => verhältnismäßige Sichtbarkeit ermitteln
        // check if object is visible
        bool IsObjectInSight(Transform obj)
        {

            var direction = obj.position - transform.position;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, direction, out RaycastHit hit, 20f))
            {
                var inFront = Vector3.Dot(direction, transform.forward);

                return hit.transform == obj && inFront > 0;
            }

            return false;
        }

        #endregion

    }
}
