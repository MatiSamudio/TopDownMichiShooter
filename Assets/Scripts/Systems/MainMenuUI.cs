using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public string firstLevelName = "LevelOne";

    public void Play()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
