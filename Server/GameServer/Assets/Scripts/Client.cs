using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class Client
{
    // Start is called before the first frame update
    public static int buffer_size = 4096;

    public int id;
    public Player player;
    public Projectile projectile;
    public ObstacleSpin obstacle_spin;
    public TCP tcp;
    public UDP udp;

    public Client(int client_id)
    {
        id = client_id;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private Packet recv_data;
        private byte[] recv_buffer;

        public TCP(int identity)
        {
            id = identity;
        }

        public void Connect(TcpClient sock)
        {
            socket = sock;
            socket.ReceiveBufferSize = buffer_size;
            socket.SendBufferSize = buffer_size;

            stream = socket.GetStream();

            recv_data = new Packet();

            recv_buffer = new byte[buffer_size];

            stream.BeginRead(recv_buffer, 0, buffer_size, RecvCallback, null);

            ServerSend.Welcome(id, "Welcome to the Game!!");
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to player {id} vias TCP: {ex}");
            }
        }

        private void RecvCallback(IAsyncResult result)
        {
            try
            {
                int byte_len = stream.EndRead(result);
                if (byte_len <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] data = new byte[byte_len];
                Array.Copy(recv_buffer, data, byte_len);

                recv_data.Reset(HandleData(data));

                stream.BeginRead(recv_buffer, 0, buffer_size, RecvCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error when receiving TCP data: {ex}");
                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packet_len = 0;

            recv_data.SetBytes(data);

            if (recv_data.UnreadLength() >= 4)
            {
                packet_len = recv_data.ReadInt();

                if (packet_len <= 0)
                {
                    return true;
                }
            }

            while (packet_len > 0 && packet_len <= recv_data.UnreadLength())
            {
                byte[] packet_byte = recv_data.ReadBytes(packet_len);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packet_byte))
                    {
                        int packet_id = packet.ReadInt();
                        Server.packet_handler[packet_id](id, packet);
                    }
                });

                /*reset packet length to zero*/
                packet_len = 0;

                if (recv_data.UnreadLength() >= 4)
                {
                    packet_len = recv_data.ReadInt();

                    if (packet_len <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packet_len <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            recv_data = null;
            recv_buffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint end_point;
        private int id;

        public UDP(int identity)
        {
            id = identity;
        }

        public void Connect(IPEndPoint end)
        {
            end_point = end;
        }

        public void SendData(Packet packet)
        {
            Server.SendUDPData(end_point, packet);
        }

        public void HandleData(Packet packet_data)
        {
            int packet_len = packet_data.ReadInt();
            byte[] packet_bytes = packet_data.ReadBytes(packet_len);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packet_bytes))
                {
                    int packet_id = packet.ReadInt();
                    Server.packet_handler[packet_id](id, packet);
                }
            });
        }

        public void Disconnect()
        {
            end_point = null;
        }
    }


    public void SendIntoGame(string player_name)
    {
        player = NetManager.instance.InstantiatePlayer();
        player.Initialise(id, player_name);

        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null)
            {
                if (client.id != id)
                {
                    ServerSend.SpawnPlayer(id, client.player);
                }
            }
        }

        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null)
            {
                ServerSend.SpawnPlayer(client.id, player);
            }
        }

        foreach(Collectible collectible in Collectible.spawners.Values)
        {
            ServerSend.CreatItemSpawner(id, collectible.spawner_id, collectible.transform.position, collectible.picked_up);
        }

    }
    public void SendObstacleIntoGame()
    {
        foreach (ObstacleSpin obstacle in ObstacleSpin.obstacles.Values)
        {
            ServerSend.CreateSpinObstacle(id, obstacle.obstacle_id, obstacle.transform.position, obstacle.transform.rotation);
        }
    }

    public void SendStaticObstacleIntoGame()
    {
        foreach(ObstacleStatic obstacle in ObstacleStatic.static_obstacle.Values)
        {
            ServerSend.CreateStaticObstacle(id, obstacle.id_, obstacle.transform.position);
        }
    }

    public void SendLevelTriggerIntoGame()
    {
        foreach(WinTrigger trigger in WinTrigger.win_condition.Values)
        {
            ServerSend.CreateWinTrigger(id, trigger.id, trigger.transform.position, trigger.transform.rotation);
        }
    }

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });


        tcp.Disconnect();
        udp.Disconnect();

        ServerSend.PlayerDisconnected(id);
    }
}
