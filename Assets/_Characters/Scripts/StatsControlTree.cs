using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;


// Why not use StatsControl? -> Unused
public class StatsControlTree : MonoBehaviour {
    public Interactable interactable;
    public Texture icon;
    public AnimationController character;
    public WeaponProperties weapon;
    public bool testTriggerCollider;
    public CharacterStats stats;
    public bool respawn;
        
	// Use this for initialization
	void Start () {
        character = GetComponent<AnimationController>();
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
        
        stats.hitpoints.current -= damage;
        if (stats.hitpoints.current > 0)
        {
            print(name + " got hit with " + damage + " dmg " + " at " + hitDirection);
            character.HandleHit(hitDirection, false);
        }
        else 
        {
            stats.hitpoints.current = 0;
            OnDestruction();
        }

        interactable.SetCanvasValues();
    }

    void OnDestruction () {
        character.HandleHit(Vector3.zero, true);
    }

    public void Spawn () {
        Spawn(stats.hitpoints.current);
    }

    public void Spawn (float initialHP) {
        character.HandleHit(Vector3.zero, false);
        stats.hitpoints.current = initialHP;
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