using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutUI : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private SaveData sd;

    public void Awake()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.LoadPlayerData();
        sd.playerData.scene = "LoadoutScene";
    }

    public void SwordButton()
    {
        sd.playerData.weapon = "0";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void SpearButton()
    {
        sd.playerData.weapon = "1";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void AxeButton()
    {
        sd.playerData.weapon = "2";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }

    public void CrossbowButton()
    {
        sd.playerData.weapon = "3";
        sd.SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }
}
