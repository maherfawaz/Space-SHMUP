using System.Collections;           // Required for Arrays & other Collections
using System.Collections.Generic;   // Required to use Lists or Dictionaries
using UnityEngine;                  // Required for Unity
using UnityEngine.SceneManagement;  // For loading & reloading of scenes

public class Main : MonoBehaviour {
    static private Main S;                        // A private singleton for Main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[]  prefabEnemies;               // Array of Enemy prefabs
    public float         enemySpawnPerSecond = 0.5f;  // # Enemies spawned/second
    public float         enemyInsetDefault = 1.5f;    // Inset from the sides
    public float         gameRestartDelay = 2;                               // a
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[] {
                             eWeaponType.blaster, eWeaponType.blaster,
                             eWeaponType.spread, eWeaponType.shield };

    private BoundsCheck  bndCheck;

    void Awake() {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this 
        // GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );                // a

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() {
        if (!spawnEnemies) {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);                     // b
        GameObject go = Instantiate<GameObject>( prefabEnemies[ ndx ] );     // c

        // Position the Enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;                                // d
        if (go.GetComponent<BoundsCheck>() != null) {                        // e
            enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius );
        }

        // Set the initial position for the spawned Enemy                    // f
        Vector3 pos = Vector3.zero;                      
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax =  bndCheck.camWidth - enemyInset;
        pos.x = Random.Range( xMin, xMax );
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );                // g
    }

    void DelayedRestart() {                                                  // c
        // Invoke the Restart() method in gameRestartDelay seconds
        Invoke( nameof(Restart), gameRestartDelay );                    
    }

    void Restart() {
        // Reload __Scene_0 to restart the game
        // "__Scene_0 below starts with 2 underscores and ends with a zero.
        SceneManager.LoadScene( "__Scene_0" );                               // d
    }

    static public void HERO_DIED() {
        S.DelayedRestart();                                                  // b
    }

    static public WeaponDefinition GET_WEAPON_DEFINITION (eWeaponType wt) {
        if (WEAP_DICT.ContainsKey(wt)) {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponDefinition());
    }

    static public void SHIP_DESTROYED(Enemy e) { 
        if (Random.value <= e.powerUpDropChance) {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);

            pUp.transform.position = e.transform.position;
        }
    }
}
