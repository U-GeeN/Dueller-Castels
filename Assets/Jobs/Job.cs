using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Job: ScriptableObject {
    
    public Color color;
    public Sprite icon;
    public string title;
    public Action action;
}

[System.Serializable] 
public class Action
{
    public Vector3 destination;
    public int actionNumber;
    public Interactable.ActionOption option;
    public int OptionInt => (int)option;
    public Interactable targetInteractable;
    public bool destinationReached = false;
    public bool isFinished = false;
    public int priority = 0; // priority for action to be ececuted
    
    public Action(
        Vector3 destination,
        int actionNumber = 0,
        Interactable.ActionOption option = Interactable.ActionOption.None,
        Interactable targetInteractable = null)
    {
        this.destination = destination;
        this.actionNumber = actionNumber;
        this.option = option;
        this.targetInteractable = targetInteractable;
    }
}