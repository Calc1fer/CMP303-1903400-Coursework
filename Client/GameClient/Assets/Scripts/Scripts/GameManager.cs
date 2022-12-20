using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager> ();
    public static Dictionary<int, Collectible> collectible = new Dictionary<int, Collectible>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, SpinObstacle> spin_obstacles = new Dictionary<int, SpinObstacle>();
    public static Dictionary<int, WinTrigger> win_trigger = new Dictionary<int, WinTrigger>();
    public static Dictionary<int, StaticObstacle> static_obstacle = new Dictionary<int, StaticObstacle>();

    public GameObject local_player_prefab;
    public GameObject player_prefab;
    public GameObject projectile_prefab;
    public GameObject collectible_prefab;
    public GameObject spin_obstacle_prefab;
    public GameObject win_trigger_prefab;
    public GameObject static_obstacle_prefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying objects...");
            Destroy(this);
        }

        for (int i = 0; i < 10; i++)
        {
            spin_obstacles.Add(i, new SpinObstacle());
        }

        for(int j = 1; j <= 2; j++)
        {
            players.Add(j, new PlayerManager());
        }

        for(int k = 0; k <= 20; k++)
        {
            static_obstacle.Add(k, new StaticObstacle());
        }

        //for(int l = 0; l <= 50; l++)
        //{
        //    projectiles.Add(l, new ProjectileManager());
        //}
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if(id == Client.instance.my_id)
        {
            player = Instantiate(local_player_prefab, position, rotation);
        }
        else
        {
            player = Instantiate(player_prefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Initialise(id, username);
        //players.Add(id, player.GetComponent<PlayerManager>());
        players[id] = player.GetComponent<PlayerManager>();
        GameManager.players[id].gameObject.SetActive(true);
    }

    public void SpawnProjectile(int id, Vector3 position)
    {
        GameObject projectile = Instantiate(projectile_prefab, position, Quaternion.identity);
        projectile.GetComponent<ProjectileManager>().Initialise(id);
        projectiles.Add(id, projectile.GetComponent<ProjectileManager>());

        //projectiles[id] = projectile.GetComponent<ProjectileManager>();
    }

    public void ItemSpawner(int spawner_id, Vector3 pos, bool picked_up)
    {
        GameObject spawner = Instantiate(collectible_prefab, pos, collectible_prefab.transform.rotation);
        spawner.GetComponent<Collectible>().Initialise(spawner_id, picked_up);
        collectible.Add(spawner_id, spawner.GetComponent<Collectible>());
    }

    public void SpinObstacleSpawner(int id, Vector3 pos, Quaternion rot)
    {
        GameObject obstacle = Instantiate(spin_obstacle_prefab, pos, rot);
        obstacle.GetComponent<SpinObstacle>().Initialise(id);
        //spin_obstacles.Add(id, obstacle.GetComponent<SpinObstacle>());

        spin_obstacles[id] = obstacle.GetComponent<SpinObstacle>();
    }

    public void StaticObstacleSpawner(int id, Vector3 pos)
    {
        GameObject obstacle = Instantiate(static_obstacle_prefab, pos, static_obstacle_prefab.transform.rotation);
        obstacle.GetComponent <StaticObstacle>().Initialise(id);

        static_obstacle[id] = obstacle.GetComponent<StaticObstacle>();
    }

    public void LevelTriggerSpawner(int id, Vector3 pos, Quaternion rot)
    {
        GameObject trigger = Instantiate(win_trigger_prefab, pos, rot);
        trigger.GetComponent<WinTrigger>().Initialise(id);
        win_trigger.Add(id, trigger.GetComponent<WinTrigger>());
    }
}
