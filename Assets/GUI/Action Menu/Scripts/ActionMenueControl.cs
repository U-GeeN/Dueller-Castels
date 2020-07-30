using System.Collections.Generic;
using UnityEngine;

public class ActionMenueControl: MonoBehaviour
{
    [SerializeField] InteractableControl interactableControl;
    [SerializeField] Interactable interactable;
    [SerializeField] private GameObject actionMenue;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<ActionButton> actionButton;
    private Job[] jobArray;
    /// Action gui position on screen
    private Vector3 guiScreenPosition;
    private Vector3 guiWorldPosition;
    private bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        interactableControl = GetComponentInParent<InteractableControl>();
        Dismiss(); 
    }

    // Update is called once per frame
    void Update()
    {
        // place menu on ground point
        if (canvasGroup.alpha > 0)
        {
            UpdateActionMenuPosition();
        }

        if (!canvasGroup.interactable && canvasGroup.alpha > 0)
        {
            //FadeOutCanvasGroup();
        }

        if (canvasGroup.interactable && canvasGroup.alpha < 1)
        {
            //FadeInCanvasGroup();
        }
    }

    private void OnEnable()
    {
        //print("OnEnable action Menu");
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }

    private void OnDisable()
    {
        if (actionButton.Count > 0)
        {

            foreach (ActionButton button in actionButton)
            {
                Destroy(button.gameObject);
            }
            actionButton = new List<ActionButton>();
        }
        guiWorldPosition = Vector3.zero;
    }

    private void UpdateActionMenuPosition()
    {
        if (guiWorldPosition == Vector3.zero)
        {
            SetGuiWorldPosition();
        }
       
        Vector3 newscreenPos = Camera.main.WorldToScreenPoint(guiWorldPosition);
        guiScreenPosition = newscreenPos;
        actionMenue.GetComponent<RectTransform>().anchoredPosition = newscreenPos;
        
    }

    private bool SetGuiWorldPosition()
    {

        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(guiScreenPosition), out RaycastHit hit, 1000.0f, layerMask, QueryTriggerInteraction.Ignore))
        {
            guiWorldPosition = hit.point;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Shows the action menue.
    /// </summary>
    /// <param name="jobArray">Jobs to be displayed in menue.</param>
    /// <param name="guiPosition">GUI position on the screen world coordinates or screen point.</param>
    public void ShowActionMenue(GameObject interactableGO, Job[] jobArray, Vector3 guiPosition, bool isWorldPosition = false)
    {
        // TODO: Store Interactable on which to execute action
        print("guiPosition " + guiPosition + " worldPos " + isWorldPosition + " actionMenue.activeSelf " + actionMenue.activeSelf);
        interactable = interactableGO.GetComponent<Interactable>();
        if (interactable == null)
        {
            //interactable = interactableGO.GetComponent<InteractableControl>();
            //TODO: make InteractableControl compatable
        }

        if (!actionMenue.activeSelf && jobArray.Length != 0)
        {
            print("guiMenu active" + actionMenue.activeSelf);
            if (isWorldPosition)
            {
                guiWorldPosition = guiPosition;
                guiScreenPosition = Camera.main.WorldToScreenPoint(guiPosition);
            }
            else
            {
                guiScreenPosition = guiPosition;
                _ = SetGuiWorldPosition();
            }

            for (int i = 0; i < jobArray.Length; i++)
            {

                Vector3 offset = GetXYDirection((Mathf.PI * 2 / jobArray.Length * i) + (Mathf.PI / 2), 70f);

                offset.z = 0;

                GameObject button = Instantiate(iconPrefab, offset, Quaternion.identity, actionMenue.transform);

                button.transform.localPosition = offset;
                ActionButton newActionButton = button.GetComponent<ActionButton>();
                newActionButton.Configure(this, jobArray[i].icon, i, jobArray[i].color, jobArray[i].title);

                actionButton.Add(newActionButton);
            }
            actionMenue.SetActive(true);
            enabled = true;
            
        }
    }

    /// <summary>
    /// Shows the action menue.
    /// </summary>
    /// <param name="isActiveJobArray">Display active Jobs if true, passive if false.</param>
    /// <param name="guiPosition">GUI position on the screen world coordinates or screen point.</param>
    public void ShowActionMenue(GameObject interactableGO, bool isActiveJobArray, Vector3 guiPosition, bool isWorldPosition = false)
    {
        // TODO: Store Interactable on which to execute action
        print("GOguiPosition " + guiPosition + " worldPos " + isWorldPosition + " actionMenue.activeSelf " + actionMenue.activeSelf);
        interactable = interactableGO.GetComponent<Interactable>();
        if (interactable == null)
        {
            //interactable = interactableGO.GetComponent<InteractableControl>();
            //TODO: make InteractableControl compatable
        }
        
        if (isActiveJobArray)
        {
            jobArray = interactable.activeJobs;
        }
        else
        {
            jobArray = interactable.passiveJobs;
        }
        
        if (jobArray.Length != 0)
        {
            if (isWorldPosition)
            {
                guiWorldPosition = guiPosition;
                guiScreenPosition = Camera.main.WorldToScreenPoint(guiPosition);
            }
            else
            {
                guiScreenPosition = guiPosition;
                _ = SetGuiWorldPosition();
            }

            for (int i = 0; i < jobArray.Length; i++)
            {

                Vector3 offset = GetXYDirection((Mathf.PI * 2 / jobArray.Length * i) + (Mathf.PI / 2), 60f);

                offset.z = 0;

                GameObject button = Instantiate(iconPrefab, offset, Quaternion.identity, actionMenue.transform);

                button.transform.localPosition = offset;
                ActionButton newActionButton = button.GetComponent<ActionButton>();
                newActionButton.Configure(this, jobArray[i].icon, i, jobArray[i].color, jobArray[i].title);

                actionButton.Add(newActionButton);
            }
            actionMenue.SetActive(true);
            enabled = true;
            
        }
    }

    // Enrypoint click on Action button
    public void SetActionNumber(int number)
    {
        // TODO: unterscheidung no/ character selected
        
        // TODO: Check which interactable should perform action
        if (interactable)
        {
            if (interactableControl.active)
            {
                print(interactableControl.active.name + " executing job nr " + number + " on " + interactable.name);
                Action action = jobArray[number].action;
                action.targetInteractable = interactable;
                action.destination = guiWorldPosition;
                action.destinationReached = false;
                interactableControl.active.PerformAction(action);
            }
        }
        

        Dismiss();
    }

    // convert Vector2 from euler to cartesian form
    Vector2 GetXYDirection(float angle, float magnitude)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
    }

    public void Dismiss()
    {
        canvasGroup.interactable = false;
        //TODO: replace this with fading animation
        canvasGroup.alpha = 0;
        OnDismissEnd();
        //---
        isFading = true;
    }

    private void FadeInCanvasGroup()
    {
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1, Time.deltaTime);//Mathf.Lerp(0, 1, canvasGroup.alpha + Time.deltaTime);
        isFading &= canvasGroup.alpha < 1;
    }

    private void FadeOutCanvasGroup()
    {
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, Time.deltaTime);
        
        if (canvasGroup.alpha >= 1)
        {
            OnDismissEnd();
        }
    }

    //Executed after fade out animation
    private void OnDismissEnd()
    {
        //print("Remove ActionButtons from Menu");
        // TODO: fade out first, create own animation for that
        if (actionButton.Count > 0)
        {

            foreach (ActionButton button in actionButton)
            {
                Destroy(button.gameObject);
            }
            actionButton = new List<ActionButton>();
        }
        guiWorldPosition = Vector3.zero;
        actionMenue.SetActive(false);
        enabled = false;
    }
}
