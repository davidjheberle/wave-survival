using UnityEngine;

public class Sweep
{
    // Hit object if there was a collision, null if none.
    public Hit Hit {
        get;
        private set;
    }

    // The furthest point the object reached along the swept path before it hit something.
    public Vector3 Position {
        get;
        private set;
    }

   // A copy of Sweep.Hit.Time, offset by epsilon, or 1 if the object didn’t hit anything during the sweep.
    public float Time {
        get;
        private set;
    }

    // h - hit of collision.
    // p - position of collision.
    // t - time of collision minus epsilon.
    public Sweep(Hit h, Vector3 p, float t = 1)
    {
        this.Hit = h;
        this.Position = p;
        this.Time = t;
    }
}
