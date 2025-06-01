using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadoutUI : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public Button spearButton;
    public Button axeButton;
    public Button crossbowButton;
    public string newSceneName;
    private SaveData sd;
    public static bool hasSpear = false;
    public static bool hasAxe = false;
    public static bool hasCrossbow = false;

    public void Awake()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.LoadPlayerData();
        sd.playerData.scene = "LoadoutScene";
        if (hasSpear)
        {
            spearButton.gameObject.SetActive(true);
        }
        if (hasAxe)
        {
            axeButton.gameObject.SetActive(true);
        }
        if (hasCrossbow)
        {
            crossbowButton.gameObject.SetActive(true);
        }
    }

    public void SwordButton()
    {
        sd.playerData.weapon = "0";
        sd.SavePlayerData();
        SceneManager.LoadScene(newSceneName);
    }

    public void SpearButton()
    {
        sd.playerData.weapon = "1";
        sd.SavePlayerData();
        SceneManager.LoadScene(newSceneName);
    }

    public void AxeButton()
    {
        sd.playerData.weapon = "2";
        sd.SavePlayerData();
        SceneManager.LoadScene(newSceneName);
    }

    public void CrossbowButton()
    {
        sd.playerData.weapon = "3";
        sd.SavePlayerData();
        SceneManager.LoadScene(newSceneName);
    }
}
