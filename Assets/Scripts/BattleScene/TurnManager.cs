using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerParent;
    [SerializeField] private GameObject enemyParent;
    [SerializeField] private Text CurrentTurn;
    [SerializeField] private Text Actions;
    private SaveData sd;
    private List<PlayerController> players = new List<PlayerController>();
    public string nextSceneName;
    public int player_actions = 3;
    public bool skipPlayerActions = false;

    void Start()
    {
        sd = gameObject.AddComponent<SaveData>();
        sd.LoadPlayerData();
        sd.SavePlayerData(null, null);

        PlayerController[] array = playerParent.GetComponentsInChildren<PlayerController>();
        players = new List<PlayerController>(array);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].index = i;
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (SceneManager.GetActiveScene().name.Equals("Level2") && GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                SceneManager.LoadScene(nextSceneName);
            }

            // —— PLAYER TURN ——
            skipPlayerActions = false;
            CurrentTurn.text = "Player Turn";
            for (int i = 0; i < player_actions; i++)
            {
                if (skipPlayerActions)
                    break;
                Actions.text = "Actions: " + (player_actions - i).ToString();
                foreach (var pc in players)
                {
                    pc.hasMoved = false;
                }
                yield return new WaitUntil(() => skipPlayerActions || players.Any(p => p.hasMoved));
            }

            foreach (Transform player in playerParent.transform)
            {
                player.GetComponent<StatsUpdater>().OnTurnEnd();
            }

            Actions.text = "Actions: 0";
            yield return new WaitForSeconds(1.5f);

            // —— ENEMY TURN ——
            // Change scene when empty
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                Debug.Log("No enemies remaining");
                SceneManager.LoadScene(nextSceneName);
            }
            CurrentTurn.text = "Enemy Turn";
            Debug.Log("TurnManager: starting enemy turn");
            foreach (Transform enemy in enemyParent.transform)
            {
                yield return StartCoroutine(enemy.gameObject.GetComponent<EnemyAI>().TakeTurn());
                yield return new WaitForSeconds(3f);
            }
            foreach (Transform enemy in enemyParent.transform)
                enemy.GetComponent<StatsUpdater>().OnTurnEnd();
            Debug.Log("TurnManager: enemy turn complete");
        }
    }
}