using UnityEngine;

public class Sweep
{
    public Hit hit;
    public Vector3 position;
    public float time;

    public Sweep()
    {
        this.hit = null;
        this.position = Vector3.zero;
        this.time = 1;
    }
}
