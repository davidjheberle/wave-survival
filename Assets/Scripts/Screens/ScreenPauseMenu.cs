using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPauseMenu : MonoBehaviour {

	public void OnClickResume()
    {
        // Set the time scale to 1.
        Time.timeScale = 1;

        // Unload the pause scene.
        GameStateLoader.UnloadSceneAsync("pause-menu");
    }

    public void OnClickMenu()
    {
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
}
