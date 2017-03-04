using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour {
    // public GameObject

    // Overrides pre and post rendering to fix up pixel positions
    private float x_, y_;
    private bool once = false;

    void OnWillRenderObject() {
        //if(!once){
            // once = true;
            // cache the _true_ position of the gameobject
            x_ = transform.position.x;
            y_ = transform.position.y;

            // Debug.Log("y_ = " + y_);

            // get int position
            int xint = (int)x_;
            int yint = (int)y_;

            // Debug.Log("yint = " + yint);

            // get fractional part of position
            float xfrac = x_ - (float)xint;
            float yfrac = y_ - (float)yint;

            // Debug.Log("yfrac = " + yfrac);

            // get pixel position, rounded
            int xpx = (int)((xfrac * 16.0f) - 0.5f);
            int ypx = (int)((yfrac * 16.0f) - 0.5f);

            // Debug.Log("ypx = " + ypx);

            // turn back into float
            float xfinal = (float)xint + ((float)xpx / 16.0f);
            float yfinal = (float)yint + ((float)ypx / 16.0f);

            // Debug.Log("yfinal = " + yfinal);
            // turned of the y-snap, it looked a bit weird
            transform.position = new Vector3(xfinal, y_, 0.0f);
        //}
    }
    void OnRenderObject() {
        // restore the true position
        transform.position = new Vector3(x_, y_, 0.0f);
    }
}
