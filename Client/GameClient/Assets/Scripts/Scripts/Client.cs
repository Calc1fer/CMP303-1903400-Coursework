using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int buffer_size = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int my_id = 0;
    public TCP tcp;
    public UDP udp;

    private bool is_connected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packet_handlers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.Log("Instance already exists, destroying objects...");
            Destroy(this);
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    public void ConnectToServer()
    {
        InitClientData();

        is_connected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet recv_data;
        private byte[] recv_buffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = buffer_size,
                SendBufferSize = buffer_size
            };

            recv_buffer = new byte[buffer_size];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if(!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            recv_data = new Packet();

            stream.BeginRead(recv_buffer, 0, buffer_size, RecvCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.Log($"Error sending data to server using TCP: {ex}");
            }
        }

        private void RecvCallback(IAsyncResult result)
        {
            try
            {
                int byte_len = stream.EndRead(result);
                if (byte_len <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byte_len];
                Array.Copy(recv_buffer, data, byte_len);

                recv_data.Reset(HandleData(data));

                stream.BeginRead(recv_buffer, 0, buffer_size, RecvCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packet_len = 0;

            recv_data.SetBytes(data);

            if(recv_data.UnreadLength() >= 4)
            {
                packet_len = recv_data.ReadInt();

                if(packet_len <= 0)
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
                        packet_handlers[packet_id](packet);
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

            if(packet_len <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            recv_data = null;
            recv_buffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint end_point;

        public UDP()
        {
            end_point = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int local_port)
        {
            socket = new UdpClient(local_port);

            socket.Connect(end_point);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }



        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.my_id);

                if(socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.Log($"Error sending data to server using UDP: {ex}");
            }
        }
        public void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref end_point);
                socket.BeginReceive(ReceiveCallback, null);

                if(data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packet_len = packet.ReadInt();
                data = packet.ReadBytes(packet_len);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packet_id = packet.ReadInt();
                    packet_handlers[packet_id](packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();

            end_point = null;
            socket = null;
        }
    }

    private void InitClientData()
    {
        packet_handlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome},
            { (int)ServerPackets.spawn_player, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.player_pos, ClientHandle.PlayerPosition },
            { (int)ServerPackets.player_rot, ClientHandle.PlayerRotation },
             { (int)ServerPackets.player_disconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.player_health, ClientHandle.PlayerHealth},
            { (int)ServerPackets.player_respawn, ClientHandle.PlayerRespawn},
            { (int)ServerPackets.item_spawner, ClientHandle.ItemSpawner},
            { (int)ServerPackets.item_spawned, ClientHandle.ItemSpawned},
             { (int)ServerPackets.item_pickedup, ClientHandle.ItemPickedUp},
             { (int)ServerPackets.spawn_projectile, ClientHandle.SpawnProjectile },
              { (int)ServerPackets.projectile_destroy, ClientHandle.DestroyProjectile},
             { (int)ServerPackets.projectile_pos, ClientHandle.ProjectilePosition },
             { (int)ServerPackets.projectile_rot, ClientHandle.ProjectileRotation },
             { (int)ServerPackets.spin_obstacle_spawner, ClientHandle.SpinObstacleSpawner },
             { (int)ServerPackets.spin_obstacle_rot, ClientHandle.SpinObstacleRotation },
             { (int)ServerPackets.level_trigger_spawner, ClientHandle.LevelTriggerSpawner },
             { (int)ServerPackets.level_won, ClientHandle.LevelWon },
             { (int)ServerPackets.static_obstacle_spawner, ClientHandle.StaticObstacleSpawner },
        };

        Debug.Log("Initialised the data packets...");
    }

    private void Disconnect()
    {
        if(is_connected)
        {
            is_connected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from the server...");
        }
    }
}
