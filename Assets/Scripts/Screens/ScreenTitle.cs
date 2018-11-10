using UnityEngine;

public class ScreenTitle : MonoBehaviour
{
    public void OnClickPlay()
    {
        // Unload the title scene.
        GameStateLoader.UnloadSceneAsync("title");

        // Load the game scene.
        GameStateLoader.LoadSceneAsync("game");
    }
}
