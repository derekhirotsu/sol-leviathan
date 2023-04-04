using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController {
    public static void ReloadCurrentScene() {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public static void LoadSceneByName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    
    public static void QuitGame() {
        Application.Quit();
    }
}
