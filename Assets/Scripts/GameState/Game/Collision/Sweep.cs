using UnityEngine;

public class Sweep
{
    public Hit hit;
    public Vector2 position;
    public float time;

    public Sweep()
    {
        this.hit = null;
        this.position = Vector2.zero;
        this.time = 1;
    }
}
