using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    private SaveData sd;

    public void StartButton()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.LoadPlayerData();
        SceneManager.LoadScene(sd.playerData.scene);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
