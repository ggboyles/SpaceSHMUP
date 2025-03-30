using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;       // the movement speed is 10m/s
    public float fireRate = 0.3f;   // seconds/shot
    public float health = 10;       // damage needed to destroy this enemy
    public int score = 100;         // points earned for destroying this
    public float powerUpDropChance = 1f; // chance to drop a PowerUp

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();

        // check if this enemy has gone off bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    // can delete
    // void OnCollisionEnter(Collision coll)
    // {
    //     GameObject otherGO = coll.gameObject;
    //     if(otherGO.GetComponent<ProjectileHero>() != null)
    //     {
    //         Destroy(otherGO);
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
    //     }

    // }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        // check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            //Only damage this enemy if its on screen
            if (bndCheck.isOnScreen)
            {
                //Get damage amount from the Main WEAP_DICT.
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if(health <= 0)
                {
                    // tell main ship was destroyed
                    if(!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    // destroy this Enemy
                    Destroy(this.gameObject);
                }
            }
            // destroy the ProjectileHero regardless
            Destroy(otherGO);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
