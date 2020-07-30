using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Utility;
using System.Collections.Generic;

public class InteractableControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CharacterCanvasProperties targetedGui;
    public CharacterCanvasProperties selectedGui;
    public ActionMenueControl actionGui;
    public SwitchCameraControl cameraSwitch;

    public Interactable targeted;
    public Interactable active;
    public Interactable controlled;

    public Job[] defaultJobs;

    [SerializeField] readonly float longClickTime = 0.15f;
    //[SerializeField] bool islongClick;
    Vector3 mousePosition3d;
    [Header("Testvariablen")]
    [SerializeField] bool selectAttached;
    public Diplomacy diplomacy;

    private void TestFunctions()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            diplomacy.AddFaction("new faction");
        }
        // Testfunction Select attached selectable
        if (SelectAttached && active != null && active.attachedChildren.Count > 0)
        {
            active.SetAsActiveInController(false);
            active.attachedChildren[0].SetAsActiveInController(true);
        }
        // 
        if (Input.GetButtonDown("Submit") && active != null)
        {
            EnterInteractable();
        }
    }

    // Use this for initialization
    void Start()
    {
        cameraSwitch = Camera.main.GetComponent<SwitchCameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        TestFunctions();

        // Left Mouse Button
        if (Input.GetButtonDown("Fire1"))
        {
            // Deselect current selected on click
            if (!isOverUI)
            {
                actionGui.Dismiss();
                
                if (targeted)
                {
                    if(targeted != active)
                    {
                        if (active)
                        {
                            active.SetAsActiveInController(false);
                        }
                        targeted.SetAsActiveInController(true);
                    }
                }
            }
        }

        // Right Mouse Button
        if (Input.GetButtonDown("Fire2"))
        {
            actionGui.Dismiss();
            // second right click
            if (IsInvoking("OnRightLongClick"))
            {
                CancelInvoke("OnRightLongClick");
                OnRightDoubleClick();
            }
            else
            {
                Invoke("OnRightLongClick", longClickTime);
            }
            
        }

        //TODO: rework in: case 1
        // Action des Selectables ausführen (zur Stelle laufen beim Character)
        if (Input.GetButtonUp("Fire2"))
        {
            // if Invoking 'OnRightLongClick' stopped, it has been already executed -> exit this function
            if (isOverUI || !IsInvoking("OnRightLongClick")) { return; }
            
            CancelInvoke("OnRightLongClick");
            print("on right short mouse up");

            // do following on right short click mouse button up
            int layerMask = 1 << 2;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, layerMask, QueryTriggerInteraction.Ignore))
            {
                mousePosition3d = hit.point;
            }

            // some Interactable is selected
            if (active)
            {
                // mouse over a inteactable
                if (targeted)
                {
                    // if shift is pressed -> add action to stack else remove previouse actions
                    if (!Input.GetButton("Run"))
                    {
                        //active.InsertAction(action);
                    }
                    
                    if (targeted.passiveJobs.Length > 0)
                    {
                        print("default action " + targeted.passiveJobs[0].action + " on " +  targeted.name);
                        Action action = targeted.passiveJobs[0].action;
                        action.targetInteractable = targeted;
                        action.isFinished = false;
                        action.destinationReached = false;
                        active.CancelActions();

                        active.PerformAction(action);
                    }
                    else
                    {
                        //Targeted has no passive jobs
                        active.CancelActions();
                        print("default action - Move to target");
                        active.PerformAction(Interactable.ActionOption.Move, targeted);
                    }
                    // TODO: check if attachment of interactables makes sense here -> perform single action vs assign interactable to action
                }
                else
                {
                    //mouse over ground
                    print("default action - Move to position");
                    // add action
                    if (!Input.GetButton("Run"))
                    {
                        
                    }
                    active.CancelActions();
                    active.PerformAction(Interactable.ActionOption.Move, mousePosition3d); 
                }
            }
        }
    }

    /// <summary>
    /// Long right mouse click handeler, invoked on right mouse button down
    /// </summary>
    private void OnRightLongClick()
    {
        if (isOverUI) { return; }
        print("OnRightLongClick");
        CancelInvoke("OnRightLongClick");

        // mouse is over Interactable
        if (targeted)
        {
            if (active)
            {
                // Mouse over active Interactable
                if (active == targeted)
                {
                    //TODO: use raycast fron getMouseButtonUp here
                    // Active + Targeted:  Select profession, view details
                    actionGui.ShowActionMenue(active.gameObject, false, active.transform.position + Vector3.up, true);
                }
                else
                {
                    // Mouse over other Interactable with active selected
                    print("longclick on other");
                    // TODO: überschneidung von active.ActivJobs und targeted.PassiveJobs anzeigen (+ active.PassiveJobs?)
                    actionGui.ShowActionMenue(targeted.gameObject, true, targeted.transform.position + Vector3.up, true);
                }
            }
            // Mouse over targeted, none selected
            else
            {
                // no active + Targeted: Action menu on floor
                print("longclick on targeted " + targeted.name + "; jobs " + targeted.activeJobs.Length);
                actionGui.ShowActionMenue(targeted.gameObject, false, targeted.transform.position + Vector3.up, true);
            }
        }
        else
        {
            if (active)
            {
                // Actions that can be done on ground (walk, defend area, build...)
                print("longclick on ground: MousePosition " + Input.mousePosition + "; jobs " + active.activeJobs.Length);
                actionGui.ShowActionMenue(active.gameObject, true, Input.mousePosition);
            }
            else
            {
                // show overview menu (view buildings, characters...)
                // create and show the menue of interactable control
                print("longclick on ground: MousePosition " + Input.mousePosition + "; jobs " + defaultJobs.Length);
                //actionGui.ShowActionMenue(gameObject, false, Input.mousePosition);
                actionGui.ShowActionMenue(gameObject, defaultJobs, Input.mousePosition);
            }
        }
    }

    private void OnRightDoubleClick()
    {
        // set high priority for default action 
    }

    public void SetInteractableAsTargeted(Interactable interactable)
    {
        targeted = interactable;
        if (interactable)
        {
            targetedGui.SetDisplayable(interactable.statsControl.stats);
        }
        else
        {
            targetedGui.SetDisplayable(null);
        }
    }

    public void SetInteractableAsActive(Interactable interactable)
    {
        active = interactable;
        if (interactable != null)
        {
            selectedGui.SetDisplayable(interactable.stats);
            selectedGui.SetButtonAction(EnterInteractable, "Take Control");
        }
        else
        {
            selectedGui.SetDisplayable(null);
        }
    }

    public void SetInteractableAsControlled(Interactable interactable)
    {
        controlled = interactable;
        if (interactable != null)
        {
            selectedGui.SetButtonAction(controlled.SetPointClickControl, "Leave");
        }
    }

    private void EnterInteractable()
    {
        if (controlled != null)
        {
            controlled.SetPointClickControl();
        }
        active.SetDirectControl();
    }

    #region Gui Funktionen

    public void UpdateGui()
    {
        if (targeted)
        {
            targetedGui.SetDisplayable(targeted.stats);
        }
        
        if (active)
        {
            selectedGui.SetDisplayable(active.stats);
        }

        if (controlled)
        {
            print("disable target gui, set selected gui");
            targetedGui.SetDisplayable(null);
            selectedGui.SetDisplayable(active.stats);
        }
    }

    #endregion

    #region pointerHandler
    public bool isOverUI;

    public bool SelectAttached { get => selectAttached; set => selectAttached = value; }

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
/*
[System.Serializable]
public class Diplomacy2
{

    public List<Faction2> factions;

    public void AddFaction(string name)
    {
        Faction2 newFaction = new Faction2
        {
            name = name
        };
        // add faction value to all factions

        factions.Add(newFaction);
        for (int i = 0; i < factions.Count; i++)
        {

            newFaction.faction[i] = 50;
        }
    }

    public struct Faction2
    {
        public string name;
        public List<int> faction;
    }
}
*/
