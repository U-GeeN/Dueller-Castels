using UnityEngine;

public class MainHandControl : MonoBehaviour
{
    public Interactable ownSelectable;
    [SerializeField] AnimationController animController;
    [SerializeField] GameObject weapon;

    // Use this for initialization
    void Start()
    {
        ownSelectable = GetComponentInParent<Interactable>();

        animController = GetComponentInParent<AnimationController>();
        animController.OnUnsheathWeapon += OnActivate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnActivate(Transform parent)
    {
        transform.SetParent(parent.transform);
        gameObject.SetActive(true);
    }
}
