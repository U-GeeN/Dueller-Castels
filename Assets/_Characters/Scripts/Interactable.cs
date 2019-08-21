using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public InteractableControl interactableControler;
    public IDisplayable stats;

    public List<Interactable> attachedChildren; // TODO: separate to Apprentices, Army...
    public List<Transform> attachedChildrenPositions;
    public int maxChildren = 1;
    public Interactable attachedParent;
    public Job[] activeJobs; //
    public Job[] passiveJobs;

    [Header("Testvariablen")]
    [SerializeField] bool isTargeted;
    [SerializeField] bool isSelected;
    [SerializeField] bool isControlled;
    [SerializeField] bool takeControl;
    [SerializeField] bool selectAttached;

    private bool IsUnderControl
    {
        set { isControlled = value; }
        get => isControlled;
    }
    public delegate void HighlightEvent(bool isActive);
    public event HighlightEvent OnHighlighted;
    public event HighlightEvent OnEnabled;

    public delegate void ActionEvent(ActionOption actionOption, Vector3 destination, Interactable target);
    public event ActionEvent OnActionExecute;

    public enum ActionOption
    {
        //Halt,   // Stop and dont move
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

    public bool IsTargeted => interactableControler.targeted == this;
    public bool IsSelected => interactableControler.active == this;
    public bool IsControlled => interactableControler.controlled == this;
    public bool IsOverUI => interactableControler.isOverUI;

    // Use this for initialization
    void Start()
    {
        stats = GetComponent<IDisplayable>();
        if (interactableControler == null)
        {
            interactableControler = GameObject.FindWithTag("Main Canvas").GetComponent<InteractableControl>();
        }
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

    #region functions
    //TODO: 
    // muss univirsell machbar sein, in einem anderen skript oder mit send messege machen
    // Function muss irgendwo anders definiert sein (CharacterControl?), hier die delegate ausführen
    public void EnterSelectable()
    {
        print("take control of " + name);
        SetAsActiveInController(false);
        interactableControler.SetControlled(this);
    }

    public void LeaveSelectable()
    {
        // muss dem owner bescheid sagen falls durch andere Umstände verlassen wurde
        SetAsActiveInController(true);
        interactableControler.SetControlled(null);
    }


    public void SwitchControl()
    {
        OnActionExecute(ActionOption.EnterInteractable, Vector3.zero, this);
        if (IsControlled)
        {
            LeaveSelectable();
        }
        else
        {
            EnterSelectable();
        }

    }

    public void AttachChild(Interactable interactable)
    {
        if (attachedChildren.Count < maxChildren) {
            attachedChildren.Add (interactable);
        }
    }
    public void DeattachChild (Interactable interactable) {
        if (attachedChildren.Contains(interactable)) {
            
            attachedChildren.Remove(interactable);
        }
    }

    #endregion

    #region Mouse Events
    // highlight + set targeted
    public void OnMouseEnter()
    {
        if (IsControlled || IsOverUI)
            return;
        Highlight(true);
    }

    // highlight + reset targeted
    public void OnMouseExit()
    {
        interactableControler.SetTargeted(null);
        Highlight(false);
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

    void Highlight(bool value)
    {
        OnHighlighted(value);
        if (value)
        {
            interactableControler.SetTargeted(this);
        }
        else
        {
            interactableControler.SetTargeted(null);
        }
    }
    /// <summary>
    /// Enables/Disables the interactable in controller.
    /// </summary>
    /// <param name="setActive">If set to <c>true</c> value.</param>
    public void SetAsActiveInController(bool setActive)
    {
        OnEnabled(setActive);
        if (setActive)
        {
            interactableControler.SetInteractableAsActive(this);
        }
        else
        {
            interactableControler.SetInteractableAsActive(null);
        }
    }


    public void PerformAction (int actionOptionNumber, Vector3 destination)
    {
        var option = (ActionOption)actionOptionNumber;
        PerformAction(option, destination);
    }

    public void PerformAction(ActionOption actionOption)
    {
        PerformAction(actionOption, Vector3.zero);
    }

    public void PerformAction(ActionOption actionOption, Vector3 destination)
    {
        print("perform action destination " + destination);
        OnActionExecute(actionOption, destination, this);
    }

    public void PerformAction(ActionOption actionOption, Interactable target)
    {
        print("set Target");
        OnActionExecute(actionOption, new Vector3(), target);
    }

    #endregion

    // an canvas über SelectableControl senden
    public void SetCanvasValues()
    {
        interactableControler.UpdateGui(stats);
    }

    #region Job functions
    public bool HasJob(ActionCollection.Option actionOption) {

        foreach (var job in activeJobs)
        {
            if (job.option == actionOption) {
                return true;
            }
        }
        return false;
    }
    #endregion

}
