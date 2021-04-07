using UnityEngine;

public class Hit
{
    public AABB collider;
    public Vector3 position;
    public Vector3 delta;
    public Vector3 normal;
    public float time;

    public Hit(AABB collider)
    {
        this.collider = collider;
        this.position = Vector3.zero;
        this.delta = Vector3.zero;
        this.normal = Vector3.zero;
        this.time = 0;
    }
}
