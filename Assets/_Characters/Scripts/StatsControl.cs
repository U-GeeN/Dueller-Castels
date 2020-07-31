using System.Runtime.InteropServices;
using UnityEngine;

public class StatsControl : MonoBehaviour
{
    Interactable interactable;
    public Texture icon;
    public AnimationController animControl;
    public WeaponProperties weapon;
    public CharacterStats stats;
    [Header("TestVariablen")]
    public bool testTriggerCollider;
    public bool respawn;
    public bool hitSelf;

    // Use this for initialization
    void Start () {
        if (GetComponent<AnimationController>())
        {
            animControl = GetComponent<AnimationController>();
            if (!weapon)
            {
                weapon = GetComponentInChildren<WeaponProperties>();   
            }
        }

        interactable = GetComponent<Interactable>();
        if (stats != null)
        {
            stats.name = interactable.name;
            InvokeRepeating("UpdateStats100", 0.0f, 0.1f);
        }
	}

    // Update is called once per frame
    void Update()
    {

        if (respawn)
        {
            respawn = false;
            Reanimate();
        }

        if (hitSelf)
        {
            hitSelf = false;
            _ = OnGotHit(stats.CalculateDamage(), new Vector3(1, 0, 0));
        }

        if (!stats.isDead && !stats.isIncapacitated)
        {

        }
	}

    private void UpdateStats100()
    {
        stats.UpdateStats();
    }

    public bool OnGotHit(Damage damage, Vector3 hitDirection)
    {
        print(name + " " + damage.ToString() + " damage is applied");
        bool isIncapacitated = stats.ApplyDamage(damage);
        if (animControl)
        {
            animControl.HandleHit(hitDirection, isIncapacitated);
        }
        // what if this interactable has no target canvas 
        interactable.SetCanvasValues();
        return isIncapacitated;
    }

    private void OnHit(ControllerColliderHit hit)
    {
        
    }

    public void Reanimate()
    {
        if (stats.isDead)
        {
            Reanimate(stats.hitpoints.max);
        }
        else
        {
            Reanimate(stats.hitpoints.current);
        }
    }

    public void Reanimate(float initialHP)
    {
        animControl.HandleHit(Vector3.zero, false);
        stats.hitpoints.current = initialHP;
        interactable.SetCanvasValues();
    }

	private void OnCollisionEnter(Collision collision)
	{
        //print("StatsControl collision enter " + name);
        /*
        WeaponProperties otherWeapon = collision.transform.GetComponent<WeaponProperties>();
        StatsControl otherStats = collision.transform.GetComponentInParent<StatsControl>();
        if (otherWeapon != null && otherStats != this)
        {
            Damage otherDamage = otherStats.stats.CalculateDamage();
            print(name + " takes " + otherWeapon.damage + " Damage from " + collision.transform.name);
            OnGotHit(otherDamage, -collision.contacts[0].normal);
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponProperties otherWeapon = other.transform.GetComponent<WeaponProperties>();
        StatsControl otherStats = other.transform.GetComponentInParent<StatsControl>();
        
        // this interactable got hit
        /*if (otherWeapon != null && otherStats != this)
        {
            Damage otherDamage = otherStats.stats.CalculateDamage();
            print(name + " takes " + otherWeapon.damage + " Damage from " + other.transform.name);
            OnGotHit(otherDamage, Vector3.forward);
        }
        */
    }
}

[System.Serializable]
public struct Damage
{
    public float rawDamage;
    public float physicalQuota;
    public float Physical => rawDamage * physicalQuota;
    public float Force => rawDamage - Physical;

    public Damage(float rawDamage, float physicalQuota)
    {
        this.rawDamage = rawDamage;
        this.physicalQuota = physicalQuota;
    }
}

[System.Serializable]
public struct Stat
{
    public float min;   //TODO: what happens if under min
    public float max;   
    public float current;
    public float replenish;
    [SerializeField]private int updatedelay;
    //public bool allwaysRegenerate;

    public Stat(float current, float max, float min = 0, float replenish = 0)//, bool allwaysRegenerate = false)
    {
        this.min = min;
        this.max = max;
        this.current = current;
        /// base regeneration ammount per second
        this.replenish = replenish;
        updatedelay = 0;
        //this.allwaysRegenerate = allwaysRegenerate;
    }

    /// <summary>
    /// updates the current value
    /// </summary>
    /// <param name="factor">boost regeneration</param>
    public void Update(float factor = 1.0f)
    {
        if (updatedelay > 0)
        {
            updatedelay -= 1;
            return;
        }
        // TODO: update negative values, while bleeding f.e.
        current = Mathf.MoveTowards(current, max, replenish / 10 * factor);
    }

    /// <summary>
    /// Drains current value from Stat. Returnes true if all points are drained.
    /// </summary>
    /// <param name="ammount">Ammount to be removed from current value</param>
    /// <returns>true if all points are drained</returns>
    public bool Drain(float ammount)
    {
        current -= ammount;
        updatedelay += Mathf.RoundToInt(ammount);
        if (current < min)
        {
            current = min;
        }
        return current <= min;
    }

    public bool ChangeValueBy(float ammount)
    {
        current += ammount;
        if (current < min)
        {
            current = min;
        }
        return current < min;
    }
}

//TODO: not interface -> base class 
[System.Serializable]
public class CharacterStats : IDisplayable
{
    public string name;
    public Stat hitpoints;
    public Stat stamina;
    public Stat damage; //should be weapon damage(multiplier)

    [SerializeField] private AnimationCurve randomCurve = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 0, 0));
    [SerializeField] internal AnimationCurve recoveryCurve;
    //TODO: add aging curve
    /// <summary>
    /// 
    /// </summary>
    public float RandomCurveValue => randomCurve.Evaluate(Random.value);
 
    public float strength = 5;
    public Texture icon;
    public bool isIncapacitated;
    public bool isDead;

    public delegate void StatEvent(bool isActive);
    public event StatEvent OnBeingIncapacitated;
    //public event StatEvent UpdateGui;

    [Header("Testvariablen")]
    public float randomCurveVal;
    public float rawDamage;

    public CharacterStats(Stat hitpoints, Stat stamina, Stat damage)
    {
        this.hitpoints = hitpoints;
        this.stamina = stamina;
        this.damage = damage;
        randomCurve = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 0, 0));
    }

    /// <summary>
    /// Damage this character will be dealing on others
    /// </summary> 
    /// /// <param name="physicalFactor">Ammount of physical damage delt, rest is force damage</param>
    public Damage CalculateDamage(float physicalFactor = 0.3f)
    {
        randomCurveVal = RandomCurveValue;

        float wholeDamage = RandomCurveValue * strength * stamina.current / 10;

        Damage myDamage = new Damage(wholeDamage, physicalFactor);
        Debug.Log("Damage: " + wholeDamage + "; phys: " + myDamage.Physical + "; force: " + myDamage.Force);
        //REMOVE LATER!!
        rawDamage = wholeDamage;

        return myDamage;
    }

    // TODO: Strength - minimum Stamina used per hit; Dexterity - ammount of extra stamina used; Less stamina - less accurate -> greater stamina consumption

    /// <summary>
    /// Apply incomming damage.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>false if no hp left</returns>
    public bool ApplyDamage(Damage damage)
    {
        isIncapacitated = stamina.Drain(damage.Force);
        isDead = hitpoints.Drain(damage.Physical);
        return isIncapacitated || isDead;
    }

    #region interface functions
    /// <summary>
    /// Updates the stats. Should be invoked every 100 ms.
    /// </summary>
    public void UpdateStats()
    {
        
        if (isDead)
        {
            stamina.current = 0;
            hitpoints.current = 0;
            OnBeingIncapacitated(true);
        }
        else
        {
            // TODO: another qualifier to prevent from allways fireing
            if (isIncapacitated)
            {
                OnBeingIncapacitated(true);
            }
            
            float staminaQuota = stamina.current / stamina.max;
            hitpoints.Update(staminaQuota);
            stamina.Update(recoveryCurve.Evaluate(staminaQuota));

            //TODO: "10" should be dependent on willpower or something
            if (isIncapacitated && stamina.current > 10 / stamina.max)
            {
                isIncapacitated = false;
                OnBeingIncapacitated(false);
            }
        }
        
    }

    public Stat Hitpoints()
    {
        return hitpoints;
    }

    public Stat Stamina()
    {
        // Display stamina = 0 if actually below zero
        if (stamina.current < 0)
        {
            Stat newStamina = stamina;
            newStamina.current = 0;
            return newStamina;
        }
        return stamina;
    }

    public Stat Damage()
    {
        return damage;
    }

    public string Name()
    {
        return "";
    }

    public Texture Icon()
    {
        return icon;
    }
    #endregion
}