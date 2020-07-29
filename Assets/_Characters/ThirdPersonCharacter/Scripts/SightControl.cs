using UnityEngine;

public class SightControl : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AI: " + other.name + " entered trigger of " + name);
        
        // Enter sight trigger
        if (other.GetComponent<Interactable>() != null && !other.isTrigger)
        {
            Interactable otherInteractable = other.GetComponent<Interactable>();
            print("watch " + other.name + " as " + otherInteractable.factionName);
            
        }
    }
}
