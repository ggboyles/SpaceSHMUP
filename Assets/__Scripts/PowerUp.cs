using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    // this is an unusual but hand use of Vector2's
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 10; // PowerUp will exist for # seconds
    public float fadeTime = 4; // then it fades over # seconds

    [Header("Dynamic")]
    public eWeaponType _type; // the type of PowerUp
    public GameObject cube; // reference to the PowerCube child
    public TextMeshPro letter; // reference to the text mesh
    public Vector3 rotPerSecond; // euler rotation speed for PowerCube
    public float birthTime; // the Time.time it was instantiated
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        // find the Cube reference (there's only a single child)
        cube = transform.GetChild(0).gameObject;
        // find the TextMesh and other components
        letter = GetComponent<TextMeshPro>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = GetComponent<Renderer>().material;

        // set a random velocity
        Vector3 vel = Random.onUnitSphere; // get Random XYZ velocity
        vel.z = 0; // flatten the vel to the XY plane
        vel.Normalize(); // normalizing a Vector3 sets its length to 1m

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.linearVelocity = vel;

        // set the rotation of this PowerUp GameObject to R:[0,0,0]
        transform.rotation = Quaternion.identity;

        // randomize rotPerSecond for PowerCube using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]),
                                   Random.Range(rotMinMax[0], rotMinMax[1]),
                                   Random.Range(rotMinMax[0], rotMinMax[1]));

        birthTime = Time.time;
    }
   
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // fade out the PowerUp over time
        // given the default values, a PowerUp will exist for 10 seconds
        // and then fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if (u > 0)
        {
            // fade the PowerCube
            Color c = cubeMat.color;
            c.a = 1f - u;               // Set the alpha of PowerCube to 1-u
            cubeMat.color = c;

            // fade the letter too, just not as much
            Color32 letterColor = letter.faceColor;
            letterColor.a = (byte)(255 * (1f - (u * 0.5f))); // Set alpha for TextMeshPro
            letter.faceColor = letterColor;
        }

        if (!bndCheck.isOnScreen)
        {
            // if PowerUp has drifted off screen, destroy it
            Destroy(gameObject);
        }
    }

    public eWeaponType type { get { return _type; } set { SetType(value); } }

    public void SetType(eWeaponType wt)
    {
        // grab the weaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor;
        // letter.color = def.color
        letter.text = def.letter;
        _type = wt;
    }

    ///<summary>
    ///This function is called by the Hero class when a PowerUp is collected.
    ///</summary>
    ///<param name="target">The GameObject absorbing this PowerUp</param>
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}