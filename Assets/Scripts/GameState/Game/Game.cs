using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    [SerializeField]
    private GameObject playerTemplate;
    [SerializeField]
    private GameObject enemyTemplate;
    [SerializeField]
    private GameObject wallTemplate;
    [SerializeField]
    private GameObject platformTemplate;

    // Self-initialize.
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    // Cross-initialize.
    private void Start() {
        var player = Instantiate(playerTemplate).GetComponent<Player>();
        player.Initialize(transform);
        var enemy = Instantiate(enemyTemplate).GetComponent<Enemy>();
        enemy.Initialize(transform, new Vector3(-3, 0, 0), Vector3.down, 10);
        var platform = Instantiate(platformTemplate).GetComponent<Platform>();
        platform.Initialize(transform, new Vector3(0, -3.5f, 0));

        // Start the wave.
    }
}
