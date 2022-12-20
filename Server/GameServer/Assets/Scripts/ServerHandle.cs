using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int from_client, Packet packet)
    {
        int client_id = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.clients[from_client].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {from_client}...");

        if (from_client != client_id)
        {
            Debug.Log($"Player \"{username}\"(ID: {from_client}) has assumed the wrong client ID ({client_id})!");
        }

        Server.clients[from_client].SendIntoGame(username);
        Server.clients[from_client].SendObstacleIntoGame();
        Server.clients[from_client].SendStaticObstacleIntoGame();
        Server.clients[from_client].SendLevelTriggerIntoGame();
    }

    public static void PlayerMovement(int from_client, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rot = packet.ReadQuaternion();

        Server.clients[from_client].player.SetInput(inputs, rot);
    }

    public static void PlayerShoot(int from_client, Packet packet)
    {
        Vector3 shoot_dir = packet.ReadVec3();

        Server.clients[from_client].player.Shoot(shoot_dir);
    }

    public static void InterpolatePos(int from_client, Packet packet)
    {
        List<Vector4>pos_vals = new List<Vector4>();

        pos_vals[0] = packet.ReadVec4();
        pos_vals[1] = packet.ReadVec4();
        pos_vals[2] = packet.ReadVec4();

        Server.clients[from_client].player.Interpolate(pos_vals);
    }
}
