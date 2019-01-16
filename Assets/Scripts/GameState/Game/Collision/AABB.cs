using UnityEngine;

public struct AABB
{
    // Center point.
    public Vector3 center;

    // Halfwidth extents along each axis.
    public Vector3 extents;

    // Min point.
    public Vector3 Min {
        get {
            return center - extents;
        }
    }

    // Max point.
    public Vector3 Max {
        get {
            return center + extents;
        }
    }

    // Size.
    public Vector3 Size {
        get {
            return extents * 2f;
        }
    }

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public AABB(Vector3 c, Vector3 e)
    {
        this.center = c;
        this.extents = e;
    }

    // Debug draw function.
    public void Draw(Color color)
    {
        Vector3 topRight = new Vector3(center.x + extents.x, center.y - extents.y);
        Vector3 topLeft = center - extents;
        Vector3 bottomRight = center + extents;
        Vector3 bottomLeft = new Vector3(center.x - extents.x, center.y + extents.y);
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }
}
