using UnityEngine;

public class ScreenPauseMenu : MonoBehaviour
{
    // Self-initialize.
    private void Awake()
    {
        Pause();
    }

    public void OnClickResume()
    {
        // Resume.
        Resume();

        // Unload the pause scene.
        GameStateLoader.UnloadSceneAsync("pause-menu");
    }

    public void OnClickMenu()
    {
        // Resume.
        Resume();

        // Quit to the main menu.
        GameStateLoader.UnloadSceneAsync("pause-menu");
        GameStateLoader.UnloadSceneAsync("game");
        GameStateLoader.LoadSceneAsync("title");
    }

    public void OnClickQuit()
    {
        // Quit application completely.
        Application.Quit();
    }

    // Resume the game.
    private static void Resume()
    {
        // Set the time scale to 1.
        Time.timeScale = 1;
    }

    // Pause the game.
    private static void Pause()
    {
        // Set the time scale to 0.
        Time.timeScale = 0;
    }
}
