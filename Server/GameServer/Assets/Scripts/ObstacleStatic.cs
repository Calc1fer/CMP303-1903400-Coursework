using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ObstacleStatic : MonoBehaviour
{
    public static Dictionary<int, ObstacleStatic>static_obstacle = new Dictionary<int, ObstacleStatic> ();
    private static int next_id = 1;

    public int id_;
    public float damage = 100f;



    private void Start()
    {
        id_ = next_id;
        next_id ++;
        static_obstacle.Add(id_, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        //compare the tag and if it is player, call the player damage function and give the damage values
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.PlayerDamage(damage);
            Debug.Log("Hit static");
        }
    }
}
