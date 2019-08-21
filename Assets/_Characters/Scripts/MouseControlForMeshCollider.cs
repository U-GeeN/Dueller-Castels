using UnityEngine;

public class MouseControlForMeshCollider : MonoBehaviour
{
    Interactable selectable;

    // Start is called before the first frame update
    void Start()
    {
        selectable = GetComponentInParent<Interactable>();
    }

    // highlight + set targeted
    private void OnMouseEnter()
    {
        selectable.OnMouseEnter();
    }

    // highlight + reset targeted
    private void OnMouseExit()
    {
        selectable.OnMouseExit();
    }

    // highlight + set Selected
    private void OnMouseUpAsButton()
    {
        selectable.OnMouseUpAsButton();
    }

    // bei Mausklick auf dieses Selectable
    private void OnMouseDown()
    {
        selectable.OnMouseDown();
    }

}
