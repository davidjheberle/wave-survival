using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Create an enemy.
    public static Enemy Create(Transform parent)
    {
        // Create a game object and add an enemy script to it.
        Enemy enemy = new GameObject("Enemy", typeof(Enemy)).GetComponent<Enemy>();
        enemy.transform.SetParent(parent);

        CollisionResolver.AddEnemy(enemy);

        return enemy;
    }

    // Speed.
    private const float SPEED = 0;

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
            return new AABB(transform.position, new Vector3(.25f / 2f, .25f / 2f, 0));
        }
    }

    private SpriteRenderer spriteRenderer;

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
            // TODO Check if the enemy has died.
            if (health <= 0)
            {
                Die();
            }
        }
    }

    // Public initialize.
    public void Initialize(Vector3 position, Vector3 direction, int health)
    {
        transform.position = position;
        Direction = direction;
        Health = health;
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the ememy sprite.
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(25, 25, 1);
        Color = Color.black;

        // Start facing down. Towards the camera.
        Direction = Vector3.down;
    }

    // Die.
    private void Die()
    {
        // Do death animation.
        Destroy(gameObject);
    }
}
