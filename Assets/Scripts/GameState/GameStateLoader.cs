using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateLoader : MonoBehaviour
{
    static GameStateLoader instance;
    
    // Self-initialize.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
            Debug.LogWarning("More than 1 GameStateLoader was initialized.");
        }
    }

    // Cross-initialize.
    void Start()
    {
        // Load the title game state is no other state is loaded.
        if (SceneManager.sceneCount == 1)
        {
            // Load the title scene.
            LoadSceneAsync("title");
        }
    }

    // Load a scene asynchronously.
    public static void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
    {
        // Load the scene.
        SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
    }

    // Unload a scene asynchronously.
    public static void UnloadSceneAsync(string sceneName)
    {
        //  Unload the scene.
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
