using UnityEngine;

public struct OBB
{
    public Vector2 c;
    public Vector3[] u;
    public Vector3 e;

    public OBB(Vector2 c, float halfLengthX, float halfLengthY, float halfLengthZ)
    {
        this.c = c;
        this.u = new Vector3[3];
        this.u[0] = Vector3.right;
        this.u[1] = Vector3.up;
        this.u[2] = Vector3.forward;
        e = new Vector3(halfLengthX, halfLengthY, halfLengthZ);
    }
}
