using UnityEngine;

public class Game : MonoBehaviour
{
    // The player.
    private Player player;

    // Self-initialize.
    private void Awake()
    {
    }

    // Cross-initialize.
    private void Start()
    {
        // Create the player.
        player = Player.Create(transform);
        player.Initialize(Vector3.zero);

        // Create an enemy.
        Enemy enemy = Enemy.Create(transform);
        enemy.Initialize(new Vector3(-3, 0, 0), Vector3.down, 10);

        // Create a wall.
        Wall wall = Wall.Create(transform);
        wall.Initialize(new Vector3(-2, -3.5f, 0));

        // Start the wave.
    }
}
