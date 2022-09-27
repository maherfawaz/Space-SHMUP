using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If you type /// in Visual Studio, it will automatically expand to a <summary>
/// <summary>
/// Checks whether a GameObject is on screen and can force it to stay on screen.
/// Note that this ONLY works for an orthographic Main Camera.
/// </summary>
public class BoundsCheck : MonoBehaviour {                                   // a
    public enum eType { center, inset, outset };

    [System.Flags]
    public enum eScreenLocs {                                                // a
        onScreen = 0,   // 0000 in binary (zero)
        offRight = 1,   // 0001 in binary
        offLeft = 2,    // 0010 in binary
        offUp = 4,      // 0100 in binary
        offDown = 8     // 1000 in binary
    }

    [Header( "Inscribed" )]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;                                         // a

    [Header( "Dynamic" )]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
    //public bool isOnScreen = true;                                         // b
    public float camWidth;
    public float camHeight;

    void Awake() {
        camHeight = Camera.main.orthographicSize;                            // b
        camWidth = camHeight * Camera.main.aspect;                           // c
    }

    void LateUpdate() {                                                      // d
        // Find the checkRadius that will enable center, inset, or outside
        float checkRadius = 0;
        if ( boundsType == eType.inset ) checkRadius = -radius;
        if ( boundsType == eType.outset ) checkRadius = radius;

        Vector3 pos = transform.position;
        screenLocs = eScreenLocs.onScreen;                                   // b
        //isOnScreen = true;                                                 // d

        if ( pos.x > camWidth + checkRadius ) {                              // e
            pos.x = camWidth + checkRadius;
            screenLocs |= eScreenLocs.offRight;                              // c
            // isOnScreen = false;                                           // a
        }
        if ( pos.x < -camWidth - checkRadius ) {
            pos.x = -camWidth - checkRadius;
            screenLocs |= eScreenLocs.offLeft;                               // c
            // isOnScreen = false;                                           // a
        }

        if ( pos.y > camHeight + checkRadius ) {
            pos.y = camHeight + checkRadius;
            screenLocs |= eScreenLocs.offUp;                                 // c
            // isOnScreen = false;                                           // a
        }
        if ( pos.y < -camHeight - checkRadius ) {
            pos.y = -camHeight - checkRadius;
            screenLocs |= eScreenLocs.offDown;                               // c
            // isOnScreen = false;                                           // a
        }

        if ( keepOnScreen && !isOnScreen ) {                                 // f
            transform.position = pos;                                        // g
            screenLocs = eScreenLocs.onScreen;
            // isOnScreen = true;                                            // a
        }
    }

    public bool isOnScreen {                                                 // e
        get { return ( screenLocs == eScreenLocs.onScreen ); }
    }

    public bool LocIs( eScreenLocs checkLoc ) {
        if ( checkLoc == eScreenLocs.onScreen ) return isOnScreen;           // a
        return ( (screenLocs & checkLoc) == checkLoc );                      // b
    }
}