using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if a GameObject is on screen and can force it to stay on screen.
/// Only works with an orthographic Main Camera.
/// </summary>

public class BoundsCheck : MonoBehaviour
{
    public enum eType {center, inset, outset};

    [System.Flags]
    public enum eScreenLocs
    {
        onScreen = 0,   // 0000 in binary
        offRight = 1,   // 0001 in binary
        offLeft = 2,    // 0010 in binary
        offUp = 4,      // 0100 in binary
        offDown = 8     // 1000 in binary
    }

    [Header("Inscribed")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic")]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
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
        screenLocs = eScreenLocs.onScreen;

        // restricts x position to camWidth
        if(pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
            screenLocs |= eScreenLocs.offRight;
        }
        if(pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
            screenLocs |= eScreenLocs.offLeft;
        }

        // restricts the y position to camHeight
        if(pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
            screenLocs |= eScreenLocs.offUp;
        }
        if(pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
            screenLocs |= eScreenLocs.offDown;
        }

        if(keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            screenLocs = eScreenLocs.onScreen;
        }
    }

    public bool isOnScreen
    {
        get {return(screenLocs == eScreenLocs.onScreen);}
    }

    public bool LocIs(eScreenLocs checkLoc)
    {
        if (checkLoc == eScreenLocs.onScreen) return isOnScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }
}
