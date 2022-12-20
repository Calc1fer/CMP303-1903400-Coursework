using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public int win_id;
    public MeshRenderer level_win_model;
    public bool win_condition = false;

    public void Initialise(int identity)
    {
        win_id = identity;
        level_win_model.enabled = true;
    }

    public void LevelWon(int player_id, int collectibles, bool win)
    {
        win_condition = win;

        if (win_condition == true)
        {
            Debug.Log($"Player {player_id} won! Score: {collectibles}");
        }
    }
}
