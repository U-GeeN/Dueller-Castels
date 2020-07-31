using UnityEngine;

public class WeaponProperties : MonoBehaviour {

    public BoxCollider attackCollider;
    public BoxCollider blockCollider;
    public int damage;      // 
    public int speed;       //berechtet sich aus weight und balance
    public float weight;
    public float balance;
    public Interactable ownInteractable;
    private AnimationController animController;

    // Use this for initialization
    void Start () {
        ownInteractable = GetComponentInParent<Interactable>();
        attackCollider = GetComponent<BoxCollider>();
        animController = GetComponentInParent<AnimationController>();
        if (animController)
        {
            animController.OnUnsheathWeapon += SetParent;
            animController.OnSheathWeapon += SetParent;
            animController.OnAttackEnterEvent += OnAttackEnter;
            animController.OnAttackExitEvent += OnAttackExit;
        }
        
    }

    //TODO: Sollte die Waffe wissen wo ihr Holster platz ist?
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void OnAttackEnter () 
    {
        print(name + " trigger on");
        attackCollider.enabled = true;
    }

    public void OnAttackExit () 
    {
        attackCollider.enabled = false;
    }

    public void OnBlockEnter()
    {
        blockCollider.enabled = true;
    }

    public void OnBlockExit()
    {
        blockCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
	{
        // other is a weapon
        if (other.GetComponent<WeaponProperties>() || other.CompareTag("BlockCollider"))
        {
            // attacke abbrechen
            print("Schwert an Schwert");
            animController.BlockHit();
            print(name + " trigger off");
            attackCollider.enabled = false;
        }
        // other is character
        StatsControl otherStats = other.GetComponent<StatsControl>();
        if (otherStats && other.GetComponent<Interactable>() != ownInteractable)
        {
            attackCollider.enabled = false;
            bool otherIsIncapacitated = otherStats.OnGotHit(ownInteractable.statsControl.stats.CalculateDamage(0.8f), Vector3.forward);
            print(name + " trigger off, other dead " + otherIsIncapacitated);
            if (otherIsIncapacitated)
            {
                // Inform Interactable that action stopped
                print("stop executing Action");
                animController.ResetToIdleState();
                ownInteractable.ActionFinished();
            }
        }
	}

    private void OnCollisionExit(Collision collision)
    {
        print(name + " trigger on");
        attackCollider.enabled = true;
    }
}
