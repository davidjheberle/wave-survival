using UnityEngine;

public class Game : MonoBehaviour
{
    // The player.
    private Player player;

    // Self-initialize.
    private void Awake()
    {
        // Create the player.
        player = Player.Create(transform);
    }

    // Cross-initialize.
    private void Start()
    {
        // Start the wave.
    }
}
