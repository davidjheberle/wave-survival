using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float WIDTH = .25f;
    private const float HEIGHT = .25f;

    private const float GRAVITY = -15;
    private const float SPEED = 0;

    private readonly Color MAIN_COLOR = Color.blue;
    private readonly Color HIT_COLOR = Color.red;

    // Create an enemy.
    public static Enemy Create(Transform parent)
    {
        // Create a game object and add an enemy script to it.
        Enemy enemy = new GameObject("Enemy", typeof(Enemy)).GetComponent<Enemy>();
        enemy.transform.SetParent(parent);

        CollisionResolver.AddEnemy(enemy);

        return enemy;
    }

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
            return new AABB(transform.position, new Vector3(WIDTH / 2f, HEIGHT / 2f, 0));
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
        spriteRenderer.transform.localScale = new Vector3(WIDTH * 100, HEIGHT * 100, 1);
        Color = MAIN_COLOR;

        // Start facing down. Towards the camera.
        Direction = Vector3.down;
    }

    // Update.
    private void Update()
    {
        // Reset enemy in case it was hit last collision cycle.
        Color = MAIN_COLOR;

        // Reset velocity.
        Vector3 velocity = Vector3.zero;

        // If not jumping.
        // Apply gravity.
        velocity.y += GRAVITY * Time.deltaTime;

        transform.Translate(velocity);

        // Check if off-screen and reset if necessary.
        CheckBounds();
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
        AABB aabb = AABB;
        Vector3 viewportMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewportMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        Vector3 newPosition = aabb.Center;

        bool adjustmentRequired = false;
        if (aabb.Min.x < viewportMin.x)
        {
            newPosition.x = viewportMin.x + aabb.Extents.x;
            adjustmentRequired = true;
        }
        else if (aabb.Max.x > viewportMax.x)
        {
            newPosition.x = viewportMax.x - aabb.Extents.x;
            adjustmentRequired = true;
        }
        if (aabb.Min.y < viewportMin.y)
        {
            newPosition.y = viewportMin.y + aabb.Extents.y;
            adjustmentRequired = true;
        }
        else if (aabb.Max.y > viewportMax.y)
        {
            newPosition.y = viewportMax.y - aabb.Extents.y;
            adjustmentRequired = true;
        }

        // Modify the transform's position if an adjustment is required.
        if (adjustmentRequired)
        {
            transform.position = newPosition;
        }
    }

    // Take damage.
    public void TakeDamage()
    {
        // Set color to hit color.
        Color = HIT_COLOR;
    }

    // Die.
    private void Die()
    {
        // Do death animation.
        Destroy(gameObject);
    }
}
