using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Create instance of the game manager
    public static GameManager instance;

    //create and initialise dictionaries so we can store the necessary data pertaining to each object required for sending info between the
    //server and client
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager> ();
    public static Dictionary<int, Collectible> collectible = new Dictionary<int, Collectible>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, SpinObstacle> spin_obstacles = new Dictionary<int, SpinObstacle>();
    public static Dictionary<int, WinTrigger> win_trigger = new Dictionary<int, WinTrigger>();
    public static Dictionary<int, StaticObstacle> static_obstacle = new Dictionary<int, StaticObstacle>();

    //fields created for attaching prefabs in the unity editor which will execute their own scripts
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

        //to avoid errors of null exceptions when working with the dictionaries, instantiate an amount of objects in
        //relevant dictionary containers so we can avoid errors. Patch work and not the most efficient way of handling this 
        //but for now it is necessary
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
    }

    //Function to spawn the player but instantiate the player object relevant to either player 1 or player 2.
    //If the id is not that of player 1 then it is player 2 so instantiate this.
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
        players[id] = player.GetComponent<PlayerManager>();
        GameManager.players[id].gameObject.SetActive(true);
    }

    //Initialise projectiles
    public void SpawnProjectile(int id, Vector3 position)
    {
        GameObject projectile = Instantiate(projectile_prefab, position, Quaternion.identity);
        projectile.GetComponent<ProjectileManager>().Initialise(id);
        projectiles.Add(id, projectile.GetComponent<ProjectileManager>());
    }

    //Initialise the collectible items
    public void ItemSpawner(int spawner_id, Vector3 pos, bool picked_up)
    {
        GameObject spawner = Instantiate(collectible_prefab, pos, collectible_prefab.transform.rotation);
        spawner.GetComponent<Collectible>().Initialise(spawner_id, picked_up);
        collectible.Add(spawner_id, spawner.GetComponent<Collectible>());
    }

    //Initialise the spinning obstacles
    public void SpinObstacleSpawner(int id, Vector3 pos, Quaternion rot)
    {
        GameObject obstacle = Instantiate(spin_obstacle_prefab, pos, rot);
        obstacle.GetComponent<SpinObstacle>().Initialise(id);
        //spin_obstacles.Add(id, obstacle.GetComponent<SpinObstacle>());

        spin_obstacles[id] = obstacle.GetComponent<SpinObstacle>();
    }

    //Initialise the static obstacles
    public void StaticObstacleSpawner(int id, Vector3 pos)
    {
        GameObject obstacle = Instantiate(static_obstacle_prefab, pos, static_obstacle_prefab.transform.rotation);
        obstacle.GetComponent <StaticObstacle>().Initialise(id);

        static_obstacle[id] = obstacle.GetComponent<StaticObstacle>();
    }

    //Initialise the end of game trigger for completing/winning the level
    public void LevelTriggerSpawner(int id, Vector3 pos, Quaternion rot)
    {
        GameObject trigger = Instantiate(win_trigger_prefab, pos, rot);
        trigger.GetComponent<WinTrigger>().Initialise(id);
        win_trigger.Add(id, trigger.GetComponent<WinTrigger>());
    }
}
