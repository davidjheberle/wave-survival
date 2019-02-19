using UnityEngine;

public struct AABB
{
    // Center point.
    public Vector3 Center {
        get;
        private set;
    }

    // Halfwidth extents along each axis.
    public Vector3 Extents {
        get;
        private set;
    }

    // Min point.
    public Vector3 Min {
        get {
            return Center - Extents;
        }
    }

    // Max point.
    public Vector3 Max {
        get {
            return Center + Extents;
        }
    }

    // Size.
    public Vector3 Size {
        get {
            return Extents * 2f;
        }
    }

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public AABB(Vector3 c, Vector3 e)
    {
        this.Center = c;
        this.Extents = e;
    }

    // Debug draw function.
    public void Draw(Color color)
    {
        Vector3 topRight = new Vector3(Center.x + Extents.x, Center.y - Extents.y);
        Vector3 topLeft = Center - Extents;
        Vector3 bottomRight = Center + Extents;
        Vector3 bottomLeft = new Vector3(Center.x - Extents.x, Center.y + Extents.y);
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }
}
