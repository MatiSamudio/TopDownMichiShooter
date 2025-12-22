using UnityEngine;

public class FinalScreenUI : MonoBehaviour
{
    [SerializeField] GameObject victoryPanel;

    void Awake()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        Time.timeScale = 0f;
        if (victoryPanel) victoryPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("Level_01"); // o "Tutorial" según tu naming real
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("MainMenu");
    }
}
