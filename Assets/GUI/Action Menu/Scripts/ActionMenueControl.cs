using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenueControl: MonoBehaviour
{
    [SerializeField] InteractableControl interactableControl;
    [SerializeField] GameObject actionMenue;
    [SerializeField] GameObject iconPrefab;
    [SerializeField] Job[] activeJobArray;
    [SerializeField] Job[] passiveJobArray;
    [SerializeField] List<ActionButton> actionButton;
    [SerializeField] Vector3 guiPosition;
    [SerializeField] bool isWorldPosition;
    public int actionNumber;
    //[Header("Testvariablen")]

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
        if (actionMenue.activeSelf)
        {
            UpdateGuiPosition();
        }
    }

    private void UpdateGuiPosition()
    {
        Vector3 screenPos = guiPosition;
        if (isWorldPosition)
        {
            screenPos = Camera.main.WorldToScreenPoint(guiPosition);
        }
        else if (screenPos == Vector3.zero)
        {
            screenPos = guiPosition;
        }

        actionMenue.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    /// <summary>
    /// Shows the action menue.
    /// </summary>
    /// <param name="jobArray">Job array to be displayed.</param>
    /// <param name="guiPosition">GUI position on the screen <see langword="async"/> world coordinates or screen point.</param>
    public void ShowActionMenue(Job[] jobArray, Vector3 guiPosition, bool isWorldPosition = true)
    {

        if (actionMenue.activeSelf || jobArray.Length == 0) return;

        this.guiPosition = guiPosition;


        for (int i = 0; i < jobArray.Length; i++)
        {

            Vector3 offset = GetXYDirection(Mathf.PI * 2 / jobArray.Length * i + Mathf.PI / 2, 40f);

            offset.z = 0;

            var button = Instantiate(iconPrefab, offset, Quaternion.identity, actionMenue.transform);

            button.transform.localPosition = offset;
            var newActionButton = button.GetComponent<ActionButton>();
            newActionButton.Configure(this, jobArray[i].icon, i, jobArray[i].color, jobArray[i].title);

            actionButton.Add(newActionButton);
        }
        actionMenue.SetActive(true);
    }

    public void SetActionNumber (int number) {
        print("executing job nr " + number + "/" + interactableControl.active.activeJobs.Length + " on " + interactableControl.active.name);
        interactableControl.active.PerformAction(number, Camera.main.WorldToScreenPoint(guiPosition));
        actionNumber = number;
        Dismiss();
    }

    public void SetActionOption (Interactable.ActionOption action)
    {
        print(name + "perform Action on gui position");
        interactableControl.active.PerformAction (action, Camera.main.WorldToScreenPoint(guiPosition));
        Dismiss();
    }

    // convert euler to cartesian form
    Vector2 GetXYDirection(float angle, float magnitude)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
    }

    public void Dismiss() {
        print("dismiss action menue");
        // TODO: fade out first
        if (actionButton.Count > 0)
        {

            foreach (ActionButton button in actionButton)
            {
                Destroy(button.gameObject);
            }


            actionButton = new List<ActionButton>();
            //actionButton.RemoveRange(0, actionButton.Count);
        }
        actionMenue.SetActive(false);
    }
}
