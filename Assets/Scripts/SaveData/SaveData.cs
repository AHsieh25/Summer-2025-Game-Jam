using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{

    [SerializeField] private PlayerData playerData = new PlayerData();

    public void SavePlayerData()
    {
        string data = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", data);

    }

    public void LoadPlayerData()
    {
        string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        playerData = JsonUtility.FromJson<PlayerData>(data);
    }
}

[System.Serializable]
public class PlayerData
{
    public string weapon;
    public string scene;
}
