using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Score : MonoBehaviour
{
    private GameObject player;
    public TextMeshProUGUI score;
    PlayerManager player_m;

    int collectibles = 0;

    public void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(player != null)
        {
            score.text = "Score: " + player.GetComponent<PlayerManager>().collectible_count.ToString();

            if(player.GetComponent<PlayerManager>().collectible_count >= player.GetComponent<PlayerManager>().max_collectibles)
            {
                score.text = "Player " + player.GetComponent<PlayerManager>().id.ToString() + " won!";
            }
        }
    }
}
