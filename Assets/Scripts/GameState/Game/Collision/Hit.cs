using UnityEngine;

public class Hit
{
    public AABB collider;
    public Vector2 position;
    public Vector2 delta;
    public Vector2 normal;
    public float time;

    public Hit(AABB collider)
    {
        this.collider = collider;
        this.position = Vector2.zero;
        this.delta = Vector2.zero;
        this.normal = Vector2.zero;
        this.time = 0;
    }
}
