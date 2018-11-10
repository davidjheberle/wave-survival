using UnityEngine.SceneManagement;

public static class GameStateLoader
{
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

    // Return true if a scene with the scene name is loaded.
    public static bool IsLoaded(string sceneName)
    {
        Scene result = SceneManager.GetSceneByName(sceneName);
        return result.IsValid() && result.isLoaded;
    }
}
