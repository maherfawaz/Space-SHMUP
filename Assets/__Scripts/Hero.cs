using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S { get; private set; }// Singleton property          // a

    [Header( "Inscribed" )]
    // These fields control the movement of the ship
    public float      speed = 30;
    public float      rollMult  = -45;
    public float      pitchMult = 30;
    public GameObject projectilePrefab;
    public float      projectileSpeed = 40;
    public Weapon[] weapons;


    [Header( "Dynamic" )]
    [SerializeField]
    [Range(0,4)]
    private float _shieldLevel = 1;  // Remember the underscore
    // public float shieldLevel = 1; // and remove this old line
    [Tooltip( "This field holds a reference to the last triggering GameObject" )]
    private GameObject lastTriggerGo = null;                                 // a
    public delegate void WeaponFireDelegate();
    public event WeaponFireDelegate fireEvent;


    void Awake() {
        if (S == null) {
            S = this; // Set the Singleton                                   // a
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        // fireEvent += TempFire;

        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);
    }

    void Update () {
        // Pull in information from the Input class
        float hAxis = Input.GetAxis("Horizontal");                           // b
        float vAxis = Input.GetAxis("Vertical");                             // b

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic                      // c
        transform.rotation = Quaternion.Euler(vAxis*pitchMult,hAxis*rollMult,0);

        if(Input.GetAxis("Jump") == 1 && fireEvent != null) {
            fireEvent();
        }
    }

    void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;                   // a
        GameObject go = rootT.gameObject;
        // Debug.Log("Shield trigger hit by: " +go.gameObject.name);         // b
        
        // Make sure it's not the same triggering go as last time
        if ( go == lastTriggerGo ) return;                                   // c
        lastTriggerGo = go;                                                  // d

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (enemy != null) {  // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // … and Destroy the enemy                 // e
        } else if (pUp != null) {
            AbsorbPowerUp(pUp);
        } else {
            Debug.Log("Shield trigger hit by non-Enemy: "+go.name);          // f
        }

    }

    public void AbsorbPowerUp(PowerUp pUp) {
        Debug.Log("Absorbed PowerUp: " + pUp.type);
        switch (pUp.type) {
        case eWeaponType.shield:
            shieldLevel++;
            break;

        default:
            if (pUp.type == weapons[0].type) { 
                Weapon weap = GetEmptyWeaponSlot();
                if (weap != null) { 
                    weap.SetType(pUp.type);
                }
            } else {
                ClearWeapons();
                weapons[0].SetType(pUp.type);
        }
        break;
    }
        pUp.AbsorbedBy(this.gameObject);
    }
    
    public float shieldLevel {
        get { return ( _shieldLevel ); }                                     // a
        private set {
            _shieldLevel = Mathf.Min( value, 4 );                            // b
            // If the shield is going to be set to less than zero
            if (value < 0) {                                                 // c
                Destroy(this.gameObject);
                Main.HERO_DIED();                                            // a
            }
        }
    }

    Weapon GetEmptyWeaponSlot() { 
        for (int i=0; i < weapons.Length; i++) { 
            if (weapons[i].type == eWeaponType.none) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() { 
        foreach (Weapon w in weapons) {
            w.SetType(eWeaponType.none);
        }
    }
}
