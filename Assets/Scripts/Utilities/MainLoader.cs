using UnityEngine;
using UnityEngine.SceneManagement;

public class MainLoader : MonoBehaviour
{
    static MainLoader instance;

    // Cross-initialize.
    void Start()
    {
        // If this hasn't run yet.
        if (instance == null)
        {
            instance = this;

            // Load the main (required) scene if it is not loaded.
            if (!GameStateLoader.IsLoaded("main"))
            {
                GameStateLoader.LoadSceneAsync("main");
            }
            // Else if it is loaded and it's the only scene loaded.
            else if (SceneManager.sceneCount == 1)
            {
                // Load the title scene.
                GameStateLoader.LoadSceneAsync("title");
            }
        }
        else
        {
            // Destroy this game object.
            Destroy(gameObject);
        }
    }
}