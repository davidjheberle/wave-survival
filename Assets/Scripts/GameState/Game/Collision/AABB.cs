using UnityEngine;

public struct AABB
{
    // Center point.
    public Vector3 c;

    // Halfwidth extents along each axis.
    public Vector3 e;

    // Min point.
    public Vector3 Min {
        get {
            return c - e;
        }
    }

    // Max point.
    public Vector3 Max {
        get {
            return c + e;
        }
    }

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public AABB(Vector3 c, Vector3 e)
    {
        this.c = c;
        this.e = e;
    }
}
