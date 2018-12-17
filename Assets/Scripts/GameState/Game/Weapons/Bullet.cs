using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    // Create the pistol.
    public static Bullet Create(Transform parent, Action<Bullet> returnBullet)
    {
        // Create a game object and add a bullet script to it.
        Bullet bullet = new GameObject("Bullet", typeof(Bullet)).GetComponent<Bullet>();
        bullet.transform.SetParent(parent);
        bullet.gameObject.AddComponent(typeof(BoxCollider2D));
        bullet.ReturnBullet = returnBullet;
        return bullet;
    }

    // Speed.
    private const float SPEED = 20;

    // Direction.
    private Vector3 Direction {
        get;
        set;
    }
    
    // Action to return bullet to the source.
    private Action<Bullet> ReturnBullet {
        get;
        set;
    }

    // Public initialize.
    public void Initialize(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        Direction = direction;
        gameObject.SetActive(true);
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

        // Check if off-screen and reset if necessary.
        CheckBounds();
    }

    // Reset bullet to ideal.
    private void Reset()
    {
        Direction = Vector3.zero;
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
        ReturnBullet(this);
    }

    // Check the position relative to the viewport.
    private void CheckBounds()
    {
        // Return if the main camera is null.
        if (Camera.main == null)
        {
            return;
        }

        // Check position relative to the viewport.
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.x < 0 || viewportPosition.x > 1 ||
            viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            Reset();
        }
    }
}
