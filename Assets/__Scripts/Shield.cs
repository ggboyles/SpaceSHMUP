using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Inscribed")]
    public float rotationPerSecond = 0.1f;

    [Header("Dynamic")]
    public int levelShown = 0;

    // this non-public variable will not appear in Inspector
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // reads current shield level from hero singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        if(levelShown != currLevel)
        {
            levelShown = currLevel;
            mat.mainTextureOffset = new Vector2(0.2f*levelShown, 0);
        }
        float rZ = -(rotationPerSecond*Time.time*360);
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
