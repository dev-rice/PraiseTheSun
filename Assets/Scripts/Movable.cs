﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
// Common utility functions and overridden behavior for movable actors
public class Movable : MonoBehaviour {
    // Overrides pre and post rendering to fix up pixel positions
    private float x_;
    private float y_;

    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    protected BoxCollider2D box_collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    protected void Start() {
        box_collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected bool IsGrounded() {
        return isTouchingGround();
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

    public Vector3 CalculateMovementAmount(Vector3 position_delta) {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (position_delta.x != 0) {
            HorizontalCollisions (ref position_delta);
        }
        if (position_delta.y != 0) {
            VerticalCollisions (ref position_delta);
        }

        return position_delta;
    }

    public bool isTouchingGround() {
        return collisions.below;
    }

    public bool isTouchingCeiling() {
        return collisions.above;
    }

    void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        float rayLength = Mathf.Abs (velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i ++) {
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

            if (hit) {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i ++) {
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = box_collider.bounds;
        bounds.Expand (skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);

    }

    void CalculateRaySpacing() {
        Bounds bounds = box_collider.bounds;
        bounds.Expand (skinWidth * -2);

        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public void Reset() {
            above = below = false;
            left = right = false;
        }
    }

}
