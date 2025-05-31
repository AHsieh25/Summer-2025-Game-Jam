using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{

    [SerializeField] private PlayerData playerData = new PlayerData();
    [SerializeField] string weapon;
    [SerializeField] string scene;

    public void SavePlayerData()
    {
        string data = JsonUtility.ToJson(playerData, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", data);

    }

    public void LoadPlayerData()
    {
        string weaponToLoad = weapon;
        string sceneToLoad = scene;
    }
}

[System.Serializable]
public class PlayerData
{
    public string currentWeapon;
    public string currentScene;
}
