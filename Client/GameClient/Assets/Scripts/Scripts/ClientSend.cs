using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.my_id);
            packet.Write(UIManager.instance.username.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.player_movement))
        {
            packet.Write(inputs.Length);

            foreach (bool input in inputs)
            {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.instance.my_id].transform.rotation);

            SendUDPData(packet);
        }
    }

    public static void InterpolateMovement(List<Vector4>pos_vals)
    {
        using (Packet packet = new Packet((int) ClientPackets.interpolate_pos))
        {
            packet.Write(pos_vals[0]);
            packet.Write(pos_vals[1]);
            packet.Write(pos_vals[2]);
        }
    }

    public static void PlayerShoot(Vector3 dir)
    {
        using (Packet packet = new Packet((int)ClientPackets.player_shoot))
        {
            packet.Write(dir);

            SendTCPData(packet);
        }
    }
    #endregion
}
