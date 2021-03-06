﻿using UnityEngine;

public class Hit
{
    // The point of contact between the two objects (or an estimation of it).
    public Vector2 Position {
        get;
        private set;
    }


    // Overlap between the two objects, and is a vector that can be added to
    // the colliding object’s position to move it back to a non-colliding
    // state.
    public Vector2 Delta {
        get;
        private set;
    }

    // Surface normal at the point of contact.
    public Vector2 Normal {
        get;
        private set;
    }

    // Only defined for segment and sweep intersections, and is fraction from
    // 0 to 1 indicating how far along the line the collision occurred.
    public float Time {
        get;
        private set;
    }

    // p - position of collision.
    // d - delta overlap of collision.
    // n - normal of the surface of collision.
    // t - time of collistion.
    public Hit(Vector2 p, Vector2 d, Vector2 n, float t = 0)
    {
        this.Position = p;
        this.Delta = d;
        this.Normal = n;
        this.Time = t;
    }
}
