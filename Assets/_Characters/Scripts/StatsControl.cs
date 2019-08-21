using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;

public class StatsControl : MonoBehaviour {
    public Interactable interactable;
    public Texture icon;
    public ThirdPersonCharacter character;
    public WeaponProperties weapon;
    public bool testTriggerCollider;
    public CharacterStats stats;
    public bool respawn;
        
	// Use this for initialization
	void Start () {
        character = GetComponent<ThirdPersonCharacter>();
        interactable = GetComponent<Interactable>();
        if (stats != null)
        {
            InvokeRepeating("UpdateStats100", 0.0f, 0.1f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
        if (respawn) {
            respawn = false;
            Spawn();
        }

	}

    private void UpdateStats100 () {
        stats.UpdateStats();
    }

    public void OnGotHit (float damage, Vector3 hitDirection) {
        
        stats.hitpoints -= damage;
        if (stats.hitpoints > 0) {
            print(name + " got hit with " + damage + " dmg " + " at " + hitDirection);
            character.HitControl(hitDirection, false);
        }
        else 
        {
            stats.hitpoints = 0;
            OnDestruction();
        }

        interactable.SetCanvasValues();
    }

    void OnDestruction () {
        character.HitControl(Vector3.zero, true);
    }

    public void Spawn () {
        Spawn(stats.hitpointsMax);
    }

    public void Spawn (float initialHP) {
        character.HitControl(Vector3.zero, false);
        stats.hitpoints = initialHP;
        interactable.SetCanvasValues();
    }

	private void OnCollisionEnter(Collision collision)
	{
        var otherDmg = collision.transform.GetComponent<WeaponProperties>();
        var otherStats = collision.transform.GetComponentInParent<StatsControl>();
        if (otherDmg != null && otherStats != this)
        {
            print(name + " takes " + otherDmg.damage + " Damage from " + collision.transform.name);
            OnGotHit(otherDmg.damage, -collision.contacts[0].normal);
            //collision.collider.isTrigger = true;
        }
	}
}


[System.Serializable]
public class CharacterStats : IDisplayable {
    
    public float hitpointsMax = 100; 
    public float hitpoints = 100;
    public float hitpointsRegen = 0.1f;
    public float staminaMax = 80;
    public float stamina = 80;
    public float staminaRegen = 0.8f;
    public float strength = 50;
    public float damage = 10;
    public Texture icon;

    /// <summary>
    /// Updates the stats. Should be invoked every 100 ms.
    /// </summary>
    public void UpdateStats ()
    {
        if (hitpoints < hitpointsMax)
        {
            hitpoints += hitpointsRegen / 10;
        }
        if (stamina < staminaMax)
        {
            stamina += staminaRegen / 10;
        }
    }



    public float Hitpoints()
    {
        return (hitpoints);
    }

    public float HitpointsMax()
    {
        return hitpointsMax;
    }

    public string Name()
    {
        return "";
    }

    public float Damage()
    {
        return damage;
    }

    public float Stamina()
    {
        return stamina;
    }

    public float StaminaMax()
    {
        return staminaMax;
    }

    public Texture Icon()
    {
        return icon;
    }

   
}