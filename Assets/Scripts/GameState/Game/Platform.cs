using UnityEngine;

public class Platform : MonoBehaviour
{
    private const float WIDTH = .25f;
    private const float HEIGHT = .25f;

    private readonly Color MAIN_COLOR = Color.gray;

    // Create a platform.
    public static Platform Create(Transform parent)
    {
        // Create a game object and add an enemy script to it.
        Platform platform = new GameObject("Platform", typeof(Platform)).GetComponent<Platform>();
        platform.transform.SetParent(parent);

        EntityTracker.AddPlatform(platform);

        return platform;
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

    // Public initialize.
    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the ememy sprite.
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(WIDTH * 100, HEIGHT * 100, 1);
        Color = MAIN_COLOR;
    }
}
