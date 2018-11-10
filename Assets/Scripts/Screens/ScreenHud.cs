using UnityEngine;

public class ScreenHud : MonoBehaviour
{
    public void OnClickMenu()
    {
        // Pause the game.
        Time.timeScale = 0;

        // Open the menu.
        GameStateLoader.LoadSceneAsync("pause-menu");
    }
}
