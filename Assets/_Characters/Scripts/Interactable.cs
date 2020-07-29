using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class Interactable : MonoBehaviour
{
    public InteractableControl interactableControler;
    public IDisplayable stats;
    public StatsControl statsControl;

    public List<Interactable> attachedTargets;  // TODO: check targets to "work" on, send to AIControl  
    public List<Interactable> attachedChildren; // TODO: separate to Apprentices, Army...
    public List<Transform> attachedChildrenPositions; // formations, if any etc.
    public int maxChildren = 1;
    public Interactable attachedParent;
    public Job[] activeJobs; //
    public Job[] passiveJobs;
    [SerializeField] private List<Action> actionStack; // current tasks TODO: + routineActionStack (main job), + IdleActionStack () 
    public Action idleAction;
    //TODO: memory with jobs/Actions that can be done at certain places. to have known places to get ressources, where home is -> attached stuff
    public int currentActionIndex = 0;


    [Header("Testvariablen")]
    [SerializeField] bool isTargeted;
    [SerializeField] bool isSelected;
    [SerializeField] bool isControlled;
    [SerializeField] bool takeControl;
    [SerializeField] bool selectAttached;
    [SerializeField] private int _factionId;
    [SerializeField] public string factionName;
    public int FactionId // inherited from parents/building on spawn
    {
        get { return _factionId; }
        set
        {
            _factionId = value;
            print("set faction id");
            factionName = GetFactionName();
        }
    }

    private bool IsUnderControl
    {
        set => isControlled = value;
        get => isControlled;
    }

    public delegate void HighlightEvent(bool isActive);
    public event HighlightEvent OnHighlighted;
    public event HighlightEvent OnSelected;

    public delegate void ActionEvent(ActionOption actionOption, Vector3 destination, Interactable target);

    public event ActionEvent OnActionExecute;
    public event ActionEvent OnEnterInteractableEvent;
    public event ActionEvent OnExitInteractableEvent;
    public delegate void GuiUpdateEvent(IDisplayable displayable);
    public event GuiUpdateEvent GuiUpdate;

    public enum ActionOption
    {
        //Halt,   // Stop ,backup action
        //Continue, //restore backup action
        None,
        Idle,   // Free to do what he wants (= Halt for now)
        Move,
        Build,
        EnterInteractable,
        ExitInteractable,
        Attach,
        Deattach,
        Harvest,
        Attack,
        Follow,
    }

    public bool IsTargeted => interactableControler ? interactableControler.targeted == this : false;
    public bool IsSelected => interactableControler ? interactableControler.active == this : false;
    public bool IsControlled => interactableControler && interactableControler.controlled == this;
    public bool IsOverUI => interactableControler.isOverUI;

    // Use this for initialization
    void Start()
    {
        if (GetComponent<StatsControl>())
        {
            
            statsControl = GetComponent<StatsControl>();
            stats = statsControl.stats;
        }
        else
        {
            stats = new CharacterStats(
                new Stat(75, 100, 0, 5),
                new Stat(75, 100, 0, 5),
                new Stat(10, 15));
        }

        idleAction = passiveJobs[0].action;

        if (interactableControler == null && GameObject.FindWithTag("Main Canvas"))
        {
            
            interactableControler = GameObject.FindWithTag("Main Canvas").GetComponent<InteractableControl>();
        }
        factionName = GetFactionName();

        //OnActionExecute(ActionOption.ExitInteractable, Vector3.zero, this);
    }

    // Update is called once per frame
    void Update()
    {
        isTargeted = IsTargeted;
        isSelected = IsSelected;
        isControlled = IsControlled;

        if (takeControl)
        {
            takeControl = false;
            SwitchControl();

        }
    }

    #region Interactable Controller functions

    /// <summary>
    /// Testfunktion zum wechseln von Control. Delete after test
    /// </summary>
    private void SwitchControl()
    {

        if (IsControlled)
        {
            SetPointClickControl();
        }
        else
        {
            SetDirectControl();
        }

    }

    /// <summary>
    /// Sets strategic control of interactables with point and click
    /// </summary>
    public void SetPointClickControl()
    {
        //TODO: muss dem owner bescheid sagen falls durch andere Umstände verlassen wurde
        SetAsActiveInController(true);
        interactableControler.SetInteractableAsControlled(null);
        Camera.main.GetComponent<SwitchCameraControl>().SetBirdViewCamera();
        OnActionExecute(ActionOption.ExitInteractable, transform.position, this);
    }

    public void SetDirectControl()
    {
        print("take control of " + name);
        SetAsActiveInController(false);
        interactableControler.SetInteractableAsControlled(this);
        Camera.main.GetComponent<SwitchCameraControl>().SetFollowCamera(transform);
        OnActionExecute(ActionOption.EnterInteractable, transform.position, this);
    }

    public void AttachChild(Interactable interactable)
    {
        if (attachedChildren.Count < maxChildren)
        {
            attachedChildren.Add(interactable);
        }
    }

    public void DeattachChild(Interactable interactable)
    {
        if (attachedChildren.Contains(interactable))
        {

            attachedChildren.Remove(interactable);
        }
    }

    // an canvas über SelectableControl senden
    public void SetCanvasValues()
    {
        interactableControler.UpdateGui();
    }

    private void SetAsTargetedInController(bool value)
    {
        OnHighlighted(value);
        if (value)
        {
            interactableControler.SetInteractableAsTargeted(this);
        }
        else
        {
            interactableControler.SetInteractableAsTargeted(null);
        }
    }

    /// <summary>
    /// Enables/Disables the interactable in interactableController.
    /// </summary>
    /// <param name="setActive">If set to <c>true</c> value.</param>
    public void SetAsActiveInController(bool setActive)
    {
        OnSelected(setActive);
        if (setActive)
        {
            interactableControler.SetInteractableAsActive(this);
        }
        else
        {
            interactableControler.SetInteractableAsActive(null);
        }
    }

    // diplomacy
    public string GetFactionName()
    {
        return interactableControler.diplomacy.GetfactionById(_factionId).name;
    }

    public int GetTrustValueTo(int factionId)
    {
        return interactableControler.diplomacy.GetTrustValue(_factionId, factionId);
    }

    #endregion

    #region Mouse Events
    // highlight + set targeted
    public void OnMouseEnter()
    {
        if (IsControlled || IsOverUI)
            return;
        SetAsTargetedInController(true);
    }

    // highlight + reset targeted
    public void OnMouseExit()
    {
        SetAsTargetedInController(false);
    }

    // highlight + set Selected
    public void OnMouseUpAsButton()
    {
        if (IsControlled || IsOverUI)
            return;

        SetAsActiveInController(true);
    }

    // bei Mausklick auf dieses Selectable
    public void OnMouseDown()
    {
        // nichts machen wenn kontrolliert
        if (IsControlled || IsOverUI)
            return;

        // deselektieren wenn selektiert
        if (IsSelected)
        {
            SetAsActiveInController(false);
        }
    }
    #endregion

   

    #region Custom Events

    // click on plane
    public void PerformAction(int actionOptionNumber, Vector3 destination)
    {
        var option = (ActionOption)actionOptionNumber;
        PerformAction(option, destination);
    }

    // click on plane
    public void PerformAction(ActionOption actionOption, Vector3 destination)
    {
        print("perform " + actionOption + " at destination " + destination);
        PerformAction(actionOption, destination, null);
    }
    // click on Interactable
    public void PerformAction(int actionOptionNumber, Interactable target)
    {
        var option = (ActionOption)actionOptionNumber;
        PerformAction(option, target);
    }
    // click on Interactable
    public void PerformAction(ActionOption actionOption, Interactable target)
    {
        print("perform " + actionOption + " at " + target.name);
        PerformAction(actionOption, Vector3.zero, target);
    }

    public void PerformAction(ActionOption actionOption, Vector3 destination, Interactable target)
    {
        Action action = new Action(destination, 0, actionOption, target);

        PerformAction(action);
    }
    public void PerformAction(Action action)
    {
        // TODO: difference between adding vs replacing action
        actionStack.Add(action);

        ExecuteNextAction();
        //OnActionExecute(job.option, job.destination, job.targetInteractable);
    }
    public void CancelActions()
    {
        actionStack.Clear();
    }

    public void InsertAction(Action action)
    {
        actionStack.Insert(0, action);
    }

    public Action GetCurrentAction()
    {
        //todo: und actionStack nicht leer
        if (actionStack.Count == 0)
        {
            return idleAction;
        }
        else
        {
            return actionStack[0];
            
        }
    }

    private void ExecuteNextAction()
    {  
        // are there actions?
        if (actionStack.Count == 0)
        {
            //TODO: resume idleActionStack
            return;
        }

        Action currentAction = GetCurrentAction();

        if (currentAction.destinationReached)
        {
            if (currentAction.option == ActionOption.Move)
            {
                ActionFinished();
            }
            else
            {
                OnActionExecute(currentAction.option, currentAction.destination, currentAction.targetInteractable);
            }
        }
        else
        {
            OnActionExecute(ActionOption.Move, currentAction.destination, currentAction.targetInteractable);
        }
       
    }

    public void TargetReached()
    {
        /// TODO: Check if action is executable (check distance / isInReach) and reset destinationreached
        GetCurrentAction().destinationReached = true;
        ExecuteNextAction();
    }

    public void ActionFinished()
    {
        GetCurrentAction().isFinished = true;
        actionStack.RemoveAt(0);
        ExecuteNextAction();
    }

    #endregion


    #region Job functions
    public bool HasJob(ActionOption actionOption)
    {

        foreach (var job in activeJobs)
        {
            if (job.action.option == actionOption)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

}
