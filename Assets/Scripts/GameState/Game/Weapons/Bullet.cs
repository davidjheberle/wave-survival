using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour {
    // Speed.
    [SerializeField]
    private float speed;

    // Direction.
    private Vector3 Direction {
        get;
        set;
    }

    // Velocity.
    public Vector3 Velocity {
        get {
            return Direction * speed;
        }
    }

    // Action to return bullet to the source.
    private Action<Bullet> ReturnBullet {
        get;
        set;
    }

    private new Rigidbody2D rigidbody;

    // Public initialize.
    public void Initialize(Transform parent, Vector3 position, Vector3 direction, Action<Bullet> returnBullet) {
        gameObject.SetActive(true);
        transform.SetParent(parent);
        transform.position = position;
        Direction = direction;
        rigidbody.velocity = Velocity;
        ReturnBullet = returnBullet;
    }

    // Self-initialize.
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Late update.
    private void LateUpdate() {
        // Check if off-screen and reset if necessary.
        CheckBounds();
    }

    // Reset bullet to idle.
    public void Reset() {
        rigidbody.velocity = Vector3.zero;
        Direction = Vector3.zero;
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
        ReturnBullet(this);
    }

    // Check the position relative to the viewport.
    private void CheckBounds() {
        // Return if the main camera is null.
        if (Camera.main == null) {
            return;
        }

        // Check position relative to the viewport.
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.x < 0 || viewportPosition.x > 1 ||
            viewportPosition.y < 0 || viewportPosition.y > 1) {
            Reset();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isActiveAndEnabled) {
            return;
        }
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy) {
            enemy.TakeDamage();
            Reset();
        }
    }
}
