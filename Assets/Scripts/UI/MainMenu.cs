using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsPanel;
    public void PlayGame()
    {
        SceneManager.LoadScene("Playground");
    }
    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }
    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
