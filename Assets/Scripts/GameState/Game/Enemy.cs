using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    // Vector that represents the direction the enemy is facing.
    private Vector3 direction;
    public Vector3 Direction {
        get {
            return direction;
        }
        private set {
            direction = value;
            // TODO Set animation group from the direction the player is facing.
        }
    }

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color mainColor;

    [SerializeField]
    private Color hitColor;

    // Color.
    public Color Color {
        get {
            return spriteRenderer.color;
        }
        set {
            spriteRenderer.color = value;
        }
    }

    // Total health of the enemy.
    private float health;
    public float Health {
        get {
            return health;
        }
        private set {
            health = value;
            if (health <= 0) {
                Die();
            }
        }
    }

    // Public initialize.
    public void Initialize(Transform parent, Vector3 position, Vector3 direction, int health) {
        transform.SetParent(parent);
        transform.position = position;
        Direction = direction;
        Health = health;
    }

    // Self-initialize.
    private void Awake() {
        // Start facing down. Towards the camera.
        Direction = Vector3.down;
    }

    // Take damage.
    Coroutine resetColor;
    public void TakeDamage() {
        Health--;

        // Set color to hit color.
        Color = hitColor;
        if (resetColor != null) {
            StopCoroutine(resetColor);
        }
        resetColor = StartCoroutine(ResetColor());
    }

    private IEnumerator ResetColor() {
        yield return new WaitForSeconds(.05f);
        Color = mainColor;
        resetColor = null;
    }

    // Die.
    private void Die() {
        // Do death animation.
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet && bullet.isActiveAndEnabled) {
            TakeDamage();
            bullet.Reset();
        }
    }
}
