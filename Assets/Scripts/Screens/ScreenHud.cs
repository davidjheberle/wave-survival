using UnityEngine;

public class ScreenHud : MonoBehaviour
{
    public void OnClickMenu()
    {
        // Open the menu and pause the game.
        GameStateLoader.LoadSceneAsync("pause-menu");
    }
}
