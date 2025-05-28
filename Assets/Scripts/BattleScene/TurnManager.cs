using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerController player1;
    [SerializeField] private PlayerController player2;
    [SerializeField] private GameObject parent;
    [SerializeField] private Text CurrentTurn;
    [SerializeField] private Text Actions;

    public int player_actions = 3;

    void Start()
    {
        player.index = 0;
        player1.index = 1;
        player2.index = 2;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // —— PLAYER TURN ——
            CurrentTurn.text = "Player Turn";
            for (int i = 0; i < player_actions; i++)
            {
                Actions.text = "Actions: " + (player_actions - i).ToString();
                player.hasMoved = false;
                player1.hasMoved = false;
                player2.hasMoved = false;
                yield return new WaitUntil(() => player.hasMoved || player1.hasMoved || player2.hasMoved);
            }
            Actions.text = "Actions: 0";

            // —— ENEMY TURN ——
            CurrentTurn.text = "Enemy Turn";
            Debug.Log("TurnManager: starting enemy turn");
            foreach (Transform child in parent.transform)
            {
                yield return StartCoroutine(child.gameObject.GetComponent<EnemyAI>().TakeTurnCoroutine());
            }
            Debug.Log("TurnManager: enemy turn complete");
        }
    }
}