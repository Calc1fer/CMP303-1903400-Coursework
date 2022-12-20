using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int my_id = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.my_id = my_id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }


    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVec3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVec3();

        if (GameManager.players[id] != null)
        {
           GameManager.players[id].transform.position = position;
        }
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        if (GameManager.players[id] != null)
        {
            GameManager.players[id].transform.rotation = rotation;
        }
    }

    public static void SpawnProjectile(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVec3();

        GameManager.instance.SpawnProjectile(id, position);
    }

    public static void ProjectilePosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVec3();

        GameManager.projectiles[id].transform.position = position;
    }

    public static void ProjectileRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.projectiles[id].transform.rotation = rotation;
    }

    public static void DestroyProjectile(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVec3();

        //if (GameManager.projectiles[id] != null)
        //{
            GameManager.projectiles[id].DestroyProjectiles(position);
//        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        //GameManager.players.Remove(id);
        //GameManager.players[id].gameObject.SetActive(false);
        GameManager.players[id] = new PlayerManager();
    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();

        GameManager.players[id].Sethealth(health);
    }

    public static void PlayerRespawn(Packet packet)
    {
        int id = packet.ReadInt();
        GameManager.players[id].Respawn();
    }

    public static void SpinObstacleSpawner(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVec3();
        Quaternion rot = packet.ReadQuaternion();

        GameManager.instance.SpinObstacleSpawner(id, pos, rot);
    }

    public static void SpinObstacleRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rot = packet.ReadQuaternion();

       // GameManager.spin_obstacles[id].Rotate(rot);
       if(GameManager.spin_obstacles[id] != null)
        {
            GameManager.spin_obstacles[id].transform.rotation = rot;
        }
    }

    public static void ItemSpawner(Packet packet)
    {
        int spawner_id = packet.ReadInt();
        Vector3 spawner_pos = packet.ReadVec3();
        bool picked_up = packet.ReadBool();

        GameManager.instance.ItemSpawner(spawner_id, spawner_pos, picked_up);
    }

    public static void ItemSpawned(Packet packet)
    {
        int spawner_id = packet.ReadInt();

        GameManager.collectible[spawner_id].ItemSpawned();
    }

    public static void ItemPickedUp(Packet packet)
    {
        int spawner_id = packet.ReadInt();
        int by_player = packet.ReadInt();

        GameManager.collectible[spawner_id].ItemPickedUp();
        GameManager.players[by_player].collectible_count++;
    }

    public static void StaticObstacleSpawner(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVec3();

        GameManager.instance.StaticObstacleSpawner(id, pos);
    }

    public static void LevelTriggerSpawner(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVec3();
        Quaternion rot = packet.ReadQuaternion();

        GameManager.instance.LevelTriggerSpawner(id, pos, rot);
    }

    public static void LevelWon(Packet packet)
    {
        int win_id = packet.ReadInt();
        int player_id = packet.ReadInt();
        int collectibles = packet.ReadInt();
        bool win = packet.ReadBool();

        GameManager.win_trigger[win_id].LevelWon(player_id, collectibles, win);
    }
}
