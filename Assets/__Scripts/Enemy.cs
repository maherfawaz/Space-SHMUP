using System.Collections;          // Required for some Arrays manipulation
using System.Collections.Generic;  // Required for Lists and Dictionaries
using UnityEngine;                 // Required for Unity

[RequireComponent( typeof(BoundsCheck) )]                                    // a
public class Enemy : MonoBehaviour {
    static public List<Enemy> ENEMIES = new List<Enemy>();

    [Header("Inscribed")]
    public float speed = 10f;      // The speed in m/s
    public float fireRate = 0.3f;  // Seconds/shot (Unused)
    public float health   = 10;    // Damage needed to destroy this enemy
    public int   score    = 100;   // Points earned for destroying this
    public float powerUpDropChance = 1f;

    [Header( "Dynamic" )]
    public int enemyCount;

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;                                            // b

    void Awake() {                                                           // c
        bndCheck = GetComponent<BoundsCheck>();
        ENEMIES.Add( this );
    }

    private void OnDestroy() {
        ENEMIES.Remove( this );
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos {                                                     // a
        get {
            return( this.transform.position );
        }
        set {
            this.transform.position = value;
        }
    }

    void Update() {
        enemyCount = ENEMIES.Count;

        Move();

        // Check whether this Enemy has gone off the bottom of the screen
        if ( bndCheck.LocIs( BoundsCheck.eScreenLocs.offDown ) ) {           // a
            Destroy( gameObject );
        }
        // if ( !bndCheck.isOnScreen ) {
        //     if ( pos.y < bndCheck.camHeight - bndCheck.radius ) {            // d
        //         // We're off the bottom, so destroy this GameObject
        //         Destroy( gameObject );
        //     }
        // }
    }

    public virtual void Move() {                                             // b
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    
    void OnCollisionEnter( Collision coll ) {
        GameObject otherGO = coll.gameObject;                                // a
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null) { 
            if (bndCheck.isOnScreen) {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health<=0) {
                    if (!calledShipDestroyed) {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    Destroy(this.gameObject);
                }
            }
            Destroy(otherGO);
        } else {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
