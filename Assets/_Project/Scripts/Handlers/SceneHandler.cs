using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    /**
    * Contains some basic methods for handling game scenes.
    */

    public void LoadSceneByName(string name) => SceneManager.LoadScene(name);

    public void LoadSceneByIndex(int index) => SceneManager.LoadScene(index);

    public void ExitGame() => Application.Quit();
}
