using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on screen.
/// Only works with an orthographic Main Camera.
/// </summary>

public class BoundsCheck : MonoBehaviour
{
    public enum eType {center, inset, outset};
    public eType boundsType = eType.center;
    public float radius = 1f;

    [Header("Dynamic")]
    public float camWidth;
    public float camHeight;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        // finds the checkRadius that will enable center, inset, or outset
        float checkRadius = 0;
        if(boundsType == eType.inset) checkRadius = -radius;
        if(boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;

        // restricts x position to camWidth
        if(pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
        }
        if(pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
        }

        // restricts the y position to camHeight
        if(pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
        }
        if(pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
        }

        transform.position = pos;
    }
}
