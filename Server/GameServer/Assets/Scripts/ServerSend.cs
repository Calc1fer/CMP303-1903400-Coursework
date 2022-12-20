using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend : MonoBehaviour
{
    private static void SendTCPData(int to_client, Packet packet)
    {
        packet.WriteLength();
        Server.clients[to_client].tcp.SendData(packet);
    }

    private static void SendUDPData(int to_client, Packet packet)
    {
        packet.WriteLength();
        Server.clients[to_client].udp.SendData(packet);
    }

    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i <= Server.max_players; i++)
        {
            Server.clients[i].tcp.SendData(packet);
        }
    }
    private static void SendTCPDataToAll(int except_client, Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i <= Server.max_players; i++)
        {
            if (i != except_client)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i <= Server.max_players; i++)
        {
            Server.clients[i].udp.SendData(packet);
        }
    }
    private static void SendUDPDataToAll(int except_client, Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i <= Server.max_players; i++)
        {
            if (i != except_client)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }
    }

    #region Packets


    //All functions are sending info to the client for being attributed to the relevant game objects in the game world.
    //Packets are filled with positions, rotations and other relevant data required for the functionality of the game
    public static void Welcome(int to_client, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(to_client);

            SendTCPData(to_client, packet);
        }
    }

    public static void SpawnPlayer(int to_client, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawn_player))
        {
            packet.Write(player.id);
            packet.Write(player.name);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            SendTCPData(to_client, packet);
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.player_pos))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.player_rot))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.id, packet);
        }
    }

    public static void SpawnProjectile(int to_client, Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawn_projectile))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.position);
            packet.Write(to_client);

            SendTCPDataToAll(packet);
        }
    }

    public static void ProjectilePosition(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectile_pos))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void ProjectileDestroyed(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectile_destroy))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void ProjectileRotation(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectile_rot))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.rotation);

            SendUDPDataToAll(projectile.id, packet);
        }
    }

    public static void PlayerDisconnected(int player_id)
    {
        using (Packet packet = new Packet((int)ServerPackets.player_disconnect))
        {
            packet.Write(player_id);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerHealth(Player player)
    {
        using (Packet packet = new Packet((int) ServerPackets.player_health))
        {
            packet.Write(player.id);
            packet.Write(player.health);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerRespawn(Player player)
    {
        using (Packet packet = new Packet((int) ServerPackets.player_respawn))
        {
            packet.Write(player.id);

            SendTCPDataToAll(packet);
        }
    }
    public static void CreateSpinObstacle(int to_client, int id, Vector3 pos, Quaternion rot)
    {
        using (Packet packet = new Packet((int)ServerPackets.spin_obstacle_spawner))
        {
            packet.Write(id);
            packet.Write(pos);
            packet.Write(rot);

            SendTCPData(to_client, packet);
        }
    }

    public static void CreateStaticObstacle(int to_client, int id, Vector3 pos)
    {
        using (Packet packet = new Packet((int)ServerPackets.static_obstacle_spawner))
        {
            packet.Write(id);
            packet.Write(pos);

            SendTCPData(to_client, packet);
        }
    }

    public static void SpinObstacleRotation(int id, Quaternion rot)
    {
        using (Packet packet = new Packet((int)ServerPackets.spin_obstacle_rot))
        {
            packet.Write(id);
            packet.Write(rot);
            SendUDPDataToAll(packet);

        }
    }

    public static void CreatItemSpawner(int to_client, int spawner_id, Vector3 spawner_pos, bool picked_up)
    {
        using (Packet packet = new Packet((int)ServerPackets.item_spawner))
        {
            packet.Write(spawner_id);
            packet.Write(spawner_pos);
            packet.Write(picked_up);

            SendTCPData(to_client, packet);
        }
    }

    public static void ItemSpawned(int spawner_id)
    {
        using (Packet packet = new Packet((int)ServerPackets.item_spawned))
        {
            packet.Write(spawner_id);
            SendTCPDataToAll(packet);
        }
    }

    public static void ItemPickedUp(int spawner_id, int by_player)
    {
        using (Packet packet = new Packet((int)ServerPackets.item_pickedup))
        {
            packet.Write(spawner_id);
            packet.Write(by_player);
            SendTCPDataToAll(packet);
        }
    }

    public static void CreateWinTrigger(int to_client, int id, Vector3 pos, Quaternion rot)
    {
        using (Packet packet = new Packet((int)ServerPackets.level_trigger_spawn))
        {
            packet.Write(id);
            packet.Write(pos);
            packet.Write(rot);

            SendTCPData(to_client, packet);
        }
    }

    public static void LevelWon(int win_id, int by_player, int collectibles, bool win)
    {
        using (Packet packet = new Packet((int)ServerPackets.level_won))
        {
            packet.Write(win_id);
            packet.Write(by_player);
            packet.Write(collectibles);
            packet.Write(win);

            SendTCPDataToAll(packet);
        }
    }
}
#endregion