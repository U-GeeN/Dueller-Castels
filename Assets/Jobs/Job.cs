using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Job: ScriptableObject {
    
    public Color color;
    public Sprite icon;
    public string title;
    public Transform destination;
    public int actionNumber;
    public ActionCollection.Option option;
    public void ExecuteAction() {
        ActionCollection.GetActionWith(option);
    }
}
