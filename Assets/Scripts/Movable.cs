using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Common utility functions and overridden behavior for movable actors
public class Movable : MonoBehaviour {
    // Overrides pre and post rendering to fix up pixel positions
    private float x_;
    private float y_;

    // const values for is grounded detection
    private const float onepixel = 0.0625f;
    private const float halfspriteheight = 0.5f;
    private const float halfspritewidth = 0.5f;
    private const int groundLayerMask = 1 << 8;

    protected bool IsGrounded() {
        Vector3 centered = new Vector2(transform.position.x,
                                       transform.position.y - halfspriteheight);

        Debug.DrawRay(centered, -Vector3.up * onepixel);
        return Physics2D.Raycast(centered, -Vector2.up, onepixel, groundLayerMask);
    }

    void OnWillRenderObject() {
        // Cache true position
        x_ = transform.position.x;
        y_ = transform.position.y;

        // get int position
        int xint = (int)x_;
        int yint = (int)y_;

        // get fractional part of position
        float xfrac = x_ - (float)xint;
        float yfrac = y_ - (float)yint;

        // get pixel position, rounded
        int xpx = (int)((xfrac * 16.0f) - 0.5f);
        int ypx = (int)((yfrac * 16.0f) - 0.5f);

        // turn back into float
        float xfinal = (float)xint + ((float)xpx / 16.0f);
        float yfinal = (float)yint + ((float)ypx / 16.0f);

        transform.position = new Vector3(xfinal, yfinal, 0.0f);
    }
    void OnRenderObject() {
        // restore the true position
        transform.position = new Vector3(x_, y_, 0.0f);
    }
}
