using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class WeaponProperties : MonoBehaviour {

    public BoxCollider attackCol;
    public BoxCollider blockCol;
    public int damage;      // 
    public int speed;       //berechtet sich aus weight und balance
    public float weight;
    public float balance;
    public Interactable ownSelectable;
    [SerializeField] ThirdPersonCharacter m_Character;

	// Use this for initialization
	void Start () {
        ownSelectable = GetComponentInParent<Interactable>();
        attackCol = GetComponent<BoxCollider>();
        m_Character = GetComponentInParent<ThirdPersonCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnAttackEnter () {
        attackCol.enabled = true;

    }

    public void OnAttackExit () {
        attackCol.enabled = false;
    }

    public void OnBlockEnter()
    {
        blockCol.enabled = true;
    }

    public void OnBlockExit()
    {
        blockCol.enabled = false;
    }

	private void OnTriggerEnter(Collider other)
	{
        // ist other eine Waffe oder Chacacter?
        if (other.GetComponent<WeaponProperties>() || other.tag == "BlockCollider")
        {
            // attacke abbrechen
            print("Schwert an Schwert");
            m_Character.BlockHit();
            print(name + " trigger off");
            //attackCol.isTrigger = false;
            attackCol.enabled = false;
        }

        StatsControl otherHp = other.GetComponent<StatsControl>();
        if (otherHp != null && other.GetComponent<Interactable>() != ownSelectable) 
        {
            print(name + " trigger off");
            //attackCol.isTrigger = false;
            attackCol.enabled = false;
        }
	}

    private void OnCollisionExit()
    {
        print(name + " trigger on");
        //attackCol.isTrigger = true;
        attackCol.enabled = true;
    }
}
