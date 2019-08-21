
using UnityEngine;
using UnityEngine.AI;

using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent Agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter Character { get; private set; } // the character we are controlling
        Camera cam;
        Interactable ownInteractable;
        public Transform primaryTarget; // target to follow
        public Transform secondaryTarget;       // ba
        public GameObject waypoint;
        public float targetDistance;
        [SerializeField] Vector3 targetPosition;
       

        [SerializeField] private float m_Turn;
        public bool followTarget;
        public BehaviourMode mode;
        //TODO: use NavMeshAgent.velocity for direct control

        /// distance character tries to hold to enemy
        public float reach;
        public bool IsInReach
        {
            get
            {
                return Vector3.SqrMagnitude (Agent.destination - transform.position) < reach * reach;
            }
        }
        /// distance to enemy at which Character prepares to fight
        public float range;
        public bool IsInRange 
        {
            get 
            {
                return Vector3.SqrMagnitude(primaryTarget.position - transform.position) < range * range;
            } 
        }
        public bool IsVisible
        {
            get
            {
                return Vector3.SqrMagnitude(primaryTarget.position - transform.position) < range * range;
            }
        }
        public bool isInRange;
        public bool isInReach;
        public bool isInSight;
        [SerializeField] List<Transform> watchlist;
        public Transform t_ManualTarget;

        public enum BehaviourMode {
            Idle,       // does stuff that he likes
            OnDuty,     // working, little interaction (Guarding...)
            Aggressive, // seeking and fighting enemies

        }

        //TODO: Waypoints - array mit targets + handler

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            Agent = GetComponentInChildren<NavMeshAgent>();
            Character = GetComponent<ThirdPersonCharacter>();
            cam = Camera.main;
            ownInteractable = GetComponentInParent<Interactable>();
            ownInteractable.OnActionExecute += OnActionExecute;


            Agent.updateRotation = false;
            Agent.updatePosition = true;
            waypoint.transform.parent = null;
            InvokeRepeating("GetVisibleObjects", 0.1f, 0.1f);
        }


        private void Update()
        {

            if (primaryTarget != null){
                isInRange = IsInRange;
                isInReach = IsInReach;
            }

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
                        Character.SetBattleMode(true);

                    }
                    if (IsInReach)
                    {
                        // Start fighting
                    }
                    if (primaryTarget != null && Agent.destination != primaryTarget.position)
                    {
                        // range länge in Richtung des characters
                        Agent.SetDestination(primaryTarget.position + (transform.position - primaryTarget.position) * reach);
                    }

                    break;
                default:
                    
                    break;
            }

            targetPosition = Agent.destination;
            targetDistance = Agent.remainingDistance;

            Character.SetMotion(Agent.velocity, false, false);
            
            m_Turn = m_Turn - transform.localEulerAngles.y;

        }

#region Action Event Functions 

        public void OnActionExecute(Interactable.ActionOption actionOption, Vector3 destination, Interactable target) {
            print(name + " performs action " + actionOption + " destination " + destination + " target " + target);
            // action does not matter, allway move to point
            // click on self -> don't move
            if (target == ownInteractable) {
                //SetWaypoint(destination, true);
            } else {
                primaryTarget = target.transform;
            }
        }

        public void OnSelectedAction()
        {
            SetTargetOnClick();
        }

        #endregion

        #region Direct Control
        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
//            float l = 0, v = 0, h = 0;
//            if (Cursor.lockState == CursorLockMode.Locked || true)
//            {
//                // read inputs
//                l = Input.GetAxis("Horizontal");
//                v = Input.GetAxis("Vertical");
//                h = Input.GetAxis("Mouse X");

//            }
//            bool crouch = Input.GetKey(KeyCode.C);

//            // we use world-relative directions in the case of no main camera
//            m_Turn = h;
//            m_Move = v * transform.forward + l * transform.right + 0 * transform.up;
//            if (t_ManualTarget != null)
//            {
//                t_ManualTarget.position = transform.position + m_Move;
//            }

//#if !MOBILE_INPUT
//            // walk speed multiplier
//            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 1.5f;
//#endif

            //// pass all parameters to the character control script
            ////m_Character.Move(m_Move, crouch, m_Jump);
            //if (m_Move != Vector3.zero)
            //{
            //    //agent.SetDestination(m_Move);
            //    //print("destination set " + m_Move + " to " + agent.destination);
            //}

            //m_Jump = false;
        }

        #endregion

        public void SetTargetOnClick () 
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1000f))
            {
                print("raycast hit " + hit.point);
                SetWaypoint(hit.point, true);
            }
        }

        private void SetWaypoint (Vector3 position, bool isWaypointActive)
        {
            print("Try Set Waypoint to " + position);

            if (NavMesh.SamplePosition(position, out NavMeshHit meshHit, 3.0f, NavMesh.AllAreas))
            {
                waypoint.transform.position = meshHit.position;
                waypoint.SetActive(isWaypointActive);
                Agent.stoppingDistance = 0.2f;
                primaryTarget = waypoint.transform;
            }
            else
            {
                print("no mesh point");
                SetTargetOnClick();
            }
        }

        void SetNewTarget (Transform newTarget) 
        {
            if (primaryTarget == null) {
                
            }
        }
        #region Collider triggers

        // SightTrigger Enter
        private void OnTriggerEnter(Collider other)
		{
            // Target erreicht
            if (other.gameObject == waypoint) 
            {
                // Target nicht mehr folgen
                SetWaypoint(waypoint.transform.position, false);
            }
            if (other.GetComponent<ThirdPersonCharacter>() != null)
            {
                watchlist.Add(other.transform);
            }
		}

        // Exit Sighttrigger
		private void OnTriggerExit(Collider other)
		{
            watchlist.Remove(other.transform);
		}
        #endregion

        #region Senses
        // Invoke every 100 ms
        void GetVisibleObjects () 
        {
            isInSight = false;
            foreach (var tr in watchlist)
            {
                if (IsObjectInSight(tr)){
                    // TODO: chooseTarget function
                    //target = tr;
                    isInSight = true;
                }
            }
        }

        // TODO: Testen: mit Physics.OverlapSphere besser?
        // Todo: gegen Ecken des skinnedMeshRenderers prüfen
        // prüft ob objekt sich hinter 
        bool IsObjectInSight (Transform obj) {
            RaycastHit hit = new RaycastHit();
            var direction = obj.position - transform.position;
            Physics.Raycast(transform.position + Vector3.up * 1.5f, direction, out hit, 20f);
            var inFront = Vector3.Dot(direction, transform.forward);

            return hit.transform == obj && inFront > 0;
        }

        #endregion
     
	}
}
