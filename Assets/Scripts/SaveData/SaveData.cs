using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public void SavePlayerData(string identifier, string weaponNum)
    {
        playerData.scene = SceneManager.GetActiveScene().name;

        if (weaponNum == null || identifier == null) return;
        
        AllyData allyData = new AllyData
        {
            name = identifier,
            weaponType = weaponNum
        };

        playerData.allies.Add(allyData);
        string data = JsonUtility.ToJson(playerData, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", data);
    }

    public void LoadPlayerData()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/SaveData.json"))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        playerData = JsonUtility.FromJson<PlayerData>(data);
    }
}

[System.Serializable]
public class PlayerData
{
    public List<AllyData> allies = new List<AllyData>();
    public string scene;
}

[System.Serializable]
public class AllyData
{
    public string name;
    public string weaponType;
}