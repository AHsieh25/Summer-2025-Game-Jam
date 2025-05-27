using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerController player1;
    [SerializeField] private PlayerController player2;
    [SerializeField] private EnemyAI enemy;
    [SerializeField] private GameObject parent;

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
            for (int i = 0; i < player_actions; i++)
            {
                player.hasMoved = false;
                player1.hasMoved = false;
                player2.hasMoved = false;
                yield return new WaitUntil(() => player.hasMoved || player1.hasMoved || player2.hasMoved);
            }

            // —— ENEMY TURN ——
            Debug.Log("TurnManager: starting enemy turn");
            foreach (Transform child in parent.transform)
            {
                yield return StartCoroutine(child.gameObject.GetComponent<EnemyAI>().TakeTurnCoroutine());
            }
            Debug.Log("TurnManager: enemy turn complete");
        }
    }
}