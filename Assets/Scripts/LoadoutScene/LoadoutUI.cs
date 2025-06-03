using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadoutUI : MonoBehaviour
{
    private static int battleNum;
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

    public void SetLoadout(string weaponID)
    {
        string playerName = "Munc";

        AllyData existing = sd.playerData.allies.Find(a => a.name == playerName);

        if (existing == null)
        {
            // If not found, create a new entry
            existing = new AllyData
            {
                name = playerName,
                weaponType = weaponID
            };
            sd.playerData.allies.Add(existing);
        }
        else
        {
            existing.weaponType = weaponID;
        }

        // Set the scene and save
        sd.SavePlayerData(playerName, weaponID);

        SceneManager.LoadScene(newSceneName);
    }

    public void SwordButton() => SetLoadout("0");
    public void SpearButton() => SetLoadout("1");
    public void AxeButton() => SetLoadout("2");
    public void CrossbowButton() => SetLoadout("3");
}
