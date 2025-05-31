using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string sceneName;

    public void StartButton()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
