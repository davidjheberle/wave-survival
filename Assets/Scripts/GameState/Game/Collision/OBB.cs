using UnityEngine;

public class OBB
{
    // Center point.
    public Vector3 c;

    // Positive halfwidth extents along each axis.
    public Vector3 e;

    // Local x, y, and z axes.
    public Vector3[] u;

    // c - center point.
    // e - positive halfwidth extents along each axis.
    public OBB(Vector3 c, Vector3 e, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
    {
        this.c = c;
        this.e = e;
        this.u = new Vector3[3];
        this.u[0] = xAxis;
        this.u[1] = yAxis;
        this.u[2] = zAxis;
    }
}
