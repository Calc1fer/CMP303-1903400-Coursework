using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleSpin : MonoBehaviour
{
    public static Dictionary<int, ObstacleSpin> obstacles = new Dictionary<int, ObstacleSpin>();
    private static int next_obstacle_id = 1;

    public int obstacle_id;
    public float damage = 100f;
    public float rotation_speed = 50f;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotation_speed * Time.deltaTime, Space.World);
        ServerSend.SpinObstacleRotation(obstacle_id, transform.rotation);
    }

    private void Start()
    {
        obstacle_id = next_obstacle_id;
        next_obstacle_id++;
        obstacles.Add(obstacle_id, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.PlayerDamage(damage);
        }
    }
}

