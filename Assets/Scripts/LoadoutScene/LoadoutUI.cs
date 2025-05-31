using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutUI : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private SaveData sd;

    public void SwordButton()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.playerData.weapon = "0";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void SpearButton()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.playerData.weapon = "1";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void AxeButton()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.playerData.weapon = "2";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void CrossbowButton()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.playerData.weapon = "3";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }
}
