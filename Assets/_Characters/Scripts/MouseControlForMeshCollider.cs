using UnityEngine;

public class MouseControlForMeshCollider : MonoBehaviour
{
    internal Interactable interactable;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponentInParent<Interactable>();
    }

    // highlight + set targeted
    private void OnMouseEnter()
    {
        interactable.OnMouseEnter();
    }

    // highlight + reset targeted
    private void OnMouseExit()
    {
        interactable.OnMouseExit();
    }

    // highlight + set Selected
    private void OnMouseUpAsButton()
    {
        //interactable.OnMouseUpAsButton();
    }

    // bei Mausklick auf dieses Selectable
    private void OnMouseDown()
    {
        //interactable.OnMouseDown();
    }

}
