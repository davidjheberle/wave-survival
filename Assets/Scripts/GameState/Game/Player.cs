using UnityEngine;

public class Player : MonoBehaviour
{
    // Speed in the x direction.
    private const float SPEED_X = 10;

    // Speed in the y direction.
    private const float SPEED_Y = 10;

    // Create the player.
    public static Player Create(Transform parent)
    {
        // Create a game object and add a player script to it.
        GameObject playerGamObject = new GameObject("Player", typeof(Player));
        playerGamObject.transform.SetParent(parent);
        return playerGamObject.GetComponent<Player>();
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the player sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(25, 25, 1);
    }

    // Update.
    private void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * SPEED_X;
        float y = Input.GetAxis("Vertical") * Time.deltaTime * SPEED_Y;
        Debug.LogFormat("x: {0}, y: {1}", x, y);
        transform.Translate(x, y, 0);

        // TODO Set animation group from the direction the player is facing.
    }
}
