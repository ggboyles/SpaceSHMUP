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

    // added for auto-fire timing (touch & mouse)
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

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

        // keyboard movement
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;

        // touch and mouse drag movement
        Vector2 tiltInput = Vector2.zero;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            pos = Vector3.Lerp(pos, worldPos, speed * Time.deltaTime);

            Vector3 delta = worldPos - transform.position;
            tiltInput = new Vector2(delta.x, delta.y);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = touch.position;
            touchPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPos);

            pos = Vector3.Lerp(pos, worldPos, speed * Time.deltaTime);

            Vector3 delta = worldPos - transform.position;
            tiltInput = new Vector2(delta.x, delta.y);
        }

        // normalizes touch/mouse tilt so behaves like axis input
        tiltInput = Vector2.ClampMagnitude(tiltInput, 1f);

        // blends keyboard and touch/mouse input
        hAxis = Mathf.Clamp(hAxis + tiltInput.x, -1f, 1f);
        vAxis = Mathf.Clamp(vAxis + tiltInput.y, -1f, 1f);

        // change transform.position based on the axes
        transform.position = pos;

        // rotates ship for dynamic feel
        transform.rotation = Quaternion.Euler(vAxis*pitchMult, hAxis*rollMult, 0);

        // allows ship to fire
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     TempFire();
        // }

        // use the fireEvent to fire Weapons when the Spacebar is pressed
        bool wantsToFire =
            Input.GetAxis("Jump") == 1 ||
            Input.GetMouseButton(0) ||
            Input.touchCount > 0;

        if(wantsToFire && fireEvent != null && Time.time >= nextFireTime)
        {
            fireEvent();
            nextFireTime = Time.time + fireRate;
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
