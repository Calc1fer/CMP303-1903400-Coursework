using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public static Dictionary<int, WinTrigger> win_condition = new Dictionary<int, WinTrigger>();
    public static int next_id = 1;

    public int id;
    public bool win = false;

    private void Start()
    {
        win = false;
        id = next_id;
        next_id++;
        win_condition.Add(id, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.GetScore() == player.max_collectibles)
            {
                win = true;
                ServerSend.LevelWon(id, player.id, player.collectibles, win);
            }
        }
    }
}
