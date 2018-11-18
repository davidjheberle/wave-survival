using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Create the pistol.
    public static Bullet Create(Vector3 spawnLocation, Vector3 direction)
    {
        // Create a game object and add a bullet script to it.
        Bullet bullet = new GameObject("Bullet", typeof(Bullet)).GetComponent<Bullet>();
        bullet.transform.position = spawnLocation;
        bullet.Direction = direction;
        return bullet;
    }

    // Speed.
    private const float SPEED = 20;

    // Direction.
    private Vector3 Direction {
        get;
        set;
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the bullet sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.color = Color.blue;
        spriteRenderer.transform.localScale = new Vector3(5, 5, 1);
    }

    // Update.
    private void Update()
    {
        Vector3 velocity = Direction * SPEED * Time.deltaTime;
        transform.Translate(velocity);
    }
}
