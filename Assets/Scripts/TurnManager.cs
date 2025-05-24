using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EnemyAI enemy;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // —— PLAYER TURN ——
            player.hasMoved = false;
            yield return new WaitUntil(() => player.hasMoved);

            // —— ENEMY TURN ——
            Debug.Log("TurnManager: starting enemy turn");
            yield return StartCoroutine(enemy.TakeTurnCoroutine());
            Debug.Log("TurnManager: enemy turn complete");
        }
    }
}