using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
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

    // Self-initialize.
    private void Awake()
    {
    }

    // Public initialize.
    public void Initialize(Transform parent, Vector3 position) {
        transform.SetParent(parent);
        transform.position = position;
    }
}
