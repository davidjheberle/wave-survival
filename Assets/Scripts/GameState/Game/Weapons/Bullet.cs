﻿using System.Collections;
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
        bullet.ReturnBullet = returnBullet;

        CollisionResolver.AddBullet(bullet);

        return bullet;
    }

    // Speed.
    private const float SPEED = 20;

    // Direction.
    private Vector3 Direction {
        get;
        set;
    }

    // Velocity.
    public Vector3 Velocity {
        get {
            return Direction * SPEED;
        }
    }

    // Collision AABB.
    public AABB AABB {
        get {
            // Create a collision AABB.
            return new AABB(transform.position, new Vector3(.05f / 2f, .05f / 2f, 0));
        }
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

    // Late update.
    private void LateUpdate()
    {
        // Move.
        transform.Translate(Velocity * Time.deltaTime);

        // Check if off-screen and reset if necessary.
        CheckBounds();
    }

    // Reset bullet to idle.
    public void Reset()
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
