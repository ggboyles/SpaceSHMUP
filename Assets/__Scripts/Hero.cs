using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S {get; private set;}

    [Header("Inscribed")]
    // these fields control movement of ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Dynamic")] [Range(0,4)]
    private float _shieldLevel = 1;
    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;
    // declaring a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // creating a WeaponFireDelegate event named fireEvent
    public event WeaponFireDelegate fireEvent;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        //fireEvent += TempFire;

        // reset the weapons to start _Hero with 1 blaster
        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);
    }

    void Update()
    {
        // pulls in info from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // rotates ship for dynamic feel
        transform.rotation = Quaternion.Euler(vAxis*pitchMult, hAxis*rollMult, 0);

        // allows ship to fire
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     TempFire();
        // }

        // use the fireEvent to fire Weapons when the Spacebar is pressed
        if(Input.GetAxis("Jump") == 1 && fireEvent != null)
        {
            fireEvent();
        }
    }

    // void TempFire()
    // {
    //     GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //     projGO.transform.position = transform.position;
    //     Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //     // rigidB.linearVelocity = Vector3.up * projectileSpeed;

    //     ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
    //     proj.type = eWeaponType.blaster;
    //     float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
    //     rigidB.linearVelocity = Vector3.up * tSpeed;
    // }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // Debug.Log("Shield trigger hit by:" + go.gameObject.name);
        
        // makes sure it's not the same triggering go as last time
        if(go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();
        if(enemy != null)
        {
            shieldLevel--; // drops shield level if hit by enemy
            Destroy(go);
        }
        else if(pUp != null)
        {
            AbsorbPowerUp(pUp);
        }
        else
        {
            Debug.LogWarning("Shield trigger hit by non-enemy" + go.name);
        }
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
        Debug.Log("Absorbed PowerUp:" + pUp.type);
        switch (pUp.type)
        {
            case eWeaponType.shield:
                shieldLevel++;
                break;

            default:
                if(pUp.type == weapons[0].type) // if same type
                {
                    Weapon weap = GetEmptyWeaponSlot();
                    if (weap != null)
                    {
                        weap.SetType(pUp.type); // set to pUp.type
                    }
                }
                else // if it is different type
                {
                    ClearWeapons();
                    weapons[0].SetType(pUp.type);
                }
                break;
        }
        pUp.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get {return (_shieldLevel);}
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if(value < 0)
            {
                Destroy(this.gameObject); // destroys Hero
                Main.HERO_DIED(); // restarts game
            }
        }
    }

    /// <summary>
    /// Finds the first empty Weapon slot (i.e., type=none) and returns it
    /// </summary>
    /// <returns>The first empty Weapon slot or null if none are empty</returns>
    
    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == eWeaponType.none)
            {
                return(weapons[i]);
            }
        }
        return(null);
    }

    /// <summary>
    /// Sets the type of all Weapon slots to none
    /// </summary>
        void ClearWeapons()
        {
            foreach(Weapon w in weapons)
            {
                w.SetType(eWeaponType.none);
            }
        }
}
