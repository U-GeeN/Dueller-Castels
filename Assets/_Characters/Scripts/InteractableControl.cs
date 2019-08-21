using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CharacterCanvasProperties targetedGui;
    public CharacterCanvasProperties selectedGui;
    public ActionMenueControl actionGui;

    public Interactable targeted;
    public Interactable active;
    public Interactable controlled;
    [SerializeField] float longClickTime = 0.1f;
    [SerializeField] bool islongClick;
    Vector3 mousePosition3d;
    [Header("Testvariablen")] 
    [SerializeField] bool selectAttached;

    // Use this for initialization
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // Select attached selectable
        if (selectAttached && active != null && active.attachedChildren.Count > 0)
        {
            active.SetAsActiveInController(false);
            active.attachedChildren[0].SetAsActiveInController(true);
        }

        // von Canvas aus usführbar machen
        if (Input.GetButtonDown("Submit") && active != null)
        {
            EnterInteractable();
        }

        if (Input.GetButtonDown("Previous"))
        {
            controlled.LeaveSelectable();
            controlled = null;
        }

        // Deselect current selected on click
        // konflikt mit OnMouseDown
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isOverUI)
            {   //muss vor selectable object "OnMouseDown" passieren
                actionGui.Dismiss();
                if (active != null)
                {
                    active.SetAsActiveInController(false);
                }
            }
        }

        // Action des Selectables ausführen (zur Stelle laufen beim Character)
        if (Input.GetButtonUp("Fire2"))
        {

            CancelInvoke("SetLongClick");

            if (!isOverUI)
            {
                int layerMask = 1 << 2;

                // This would cast rays only against colliders in layer 8.
                // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                layerMask = ~layerMask;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000.0f, layerMask, QueryTriggerInteraction.Ignore))
                {
                    mousePosition3d = hit.point;
                }

                if (active != null)
                {

                    // attach Selected to Targeted (follow, Work on, Fight...)
                    if (targeted != null)
                    {
                        print("default action - Move to target");
                        active.PerformAction(Interactable.ActionOption.Move, targeted);
                        // check if allways neccessary, or should be done in interactable
                        targeted.AttachChild(active);
                    }
                    else
                    {
                        print("default action - Move to position");
                        // dont execute if longclick
                        if (!islongClick)
                        {
                            // TODO: make active perform default action
                            active.PerformAction(Interactable.ActionOption.Move, mousePosition3d);
                        }
                    }
                }
            }
            islongClick = false;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (!islongClick)
            {
                Invoke("SetLongClick", longClickTime);
            }
        }

        if (islongClick && !isOverUI)
        {
            islongClick = false;

            if (targeted)
            {
                if (active)
                {
                    if (active == targeted)
                    {
                        // Active + Targeted:  Select profession, view details
                        print("longclick on self");
                        actionGui.ShowActionMenue(active.passiveJobs, active.transform.position + Vector3.up);
                    }
                    else 
                    {
                        // 
                        print("longclick on other");
                        // TODO: überschneidung von active.ActivJobs und targeted.PassiveJobs anzeigen (+ active.PassiveJobs?)
                        actionGui.ShowActionMenue(targeted.passiveJobs, targeted.transform.position + Vector3.up);
                    }
                }
                // 
                else
                {
                    // no active + Targeted: Action menu on floor
                    print("longclick on targeted " + targeted.name + "; jobs " + active.activeJobs.Length);
                    actionGui.ShowActionMenue(targeted.activeJobs, mousePosition3d);
                }
            }
            else
            {
                if (active)
                {
                    // Actions that can be done on ground (walk, defend area, build...)
                    print("longclick on ground: MousePosition " + Input.mousePosition + "; jobs " + active.activeJobs.Length);
                    actionGui.ShowActionMenue(active.activeJobs, Input.mousePosition, false);
                }
                else 
                {
                    // show overview menu (view buildings, characters...)
                }
            }
        }
    }

    void SetLongClick () {
        islongClick = true;
    }

    void DismissActionGui() {
        actionGui.Dismiss();
    }

    public void SetTargeted(Interactable selectable)
    {
        targeted = selectable;
        if (selectable != null)
        {
            SetGuiProperties(targetedGui, selectable.stats);

        }
    }

    public void SetInteractableAsActive(Interactable interactable)
    {
        active = interactable;
        if (interactable != null)
        {
            SetGuiProperties(selectedGui, interactable.stats);
            selectedGui.SetButtonAction(EnterInteractable, "Take Control");
        }
    }


    public void SetControlled(Interactable selectable)
    {
        controlled = selectable;
        // TODO: abfrage ob eigener Selectable
        if (selectable != null)
        {
            selectedGui.SetButtonAction(controlled.LeaveSelectable, "Leave");
        }
    }

    private void EnterInteractable ()
    {
        if (controlled != null)
        {
            controlled.LeaveSelectable();
        }
        active.EnterSelectable();
    }

    #region Gui Funktionen

    public void UpdateGui(IDisplayable stats = null)
    {
        if (targeted)
        {
            UpdateTargetedGui(stats);
        }
        if (active)
        {
            UpdateSelectedGui(stats);
        }
        if (controlled)
        {
            UpdateTargetedGui();
            UpdateSelectedGui(stats);
        }
    }

    public void UpdateTargetedGui(IDisplayable stats = null)
    {
        SetGuiProperties(targetedGui, stats);
    }

    public void UpdateSelectedGui(IDisplayable stats = null)
    {
        SetGuiProperties(selectedGui, stats);
    }

    public void SetGuiProperties(CharacterCanvasProperties gui, IDisplayable stats = null)
    {
        if (stats != null)
        {
            gui.SetDisplayable(stats);
        }
        else
        {
            gui.SetProperties();
        }
    }
    #endregion

    #region pointerHandler
    public bool isOverUI = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverUI = false;
    }
    #endregion
}