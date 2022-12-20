using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class Server
{
    public static int max_players { get; private set; }
    public static int port { get; private set; }

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

    public delegate void PacketHandler(int from_client, Packet packet);
    public static Dictionary<int, PacketHandler> packet_handler;

    public static TcpListener tcp_listener;
    public static UdpClient udp_listener;

    public static void Start(int max_p, int p)
    {
        max_players = max_p;
        port = p;

        Debug.Log("Server Launching...");
        InitServerData();

        tcp_listener = new TcpListener(IPAddress.Any, port);
        tcp_listener.Start();
        tcp_listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udp_listener = new UdpClient(port);
        udp_listener.BeginReceive(UDPReceiveCallback, null);


        Debug.Log($"Server Port {port}. ");
    }

    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient client = tcp_listener.EndAcceptTcpClient(result);
        tcp_listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= max_players; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(client);
                return;
            }
        }

        Debug.Log($"{client.Client.RemoteEndPoint}Server is full. Failed to connect.");
    }

    private static void UDPReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint client_end_point = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp_listener.EndReceive(result, ref client_end_point);
            udp_listener.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4)
            {
                return;
            }

            using (Packet packet = new Packet(data))
            {
                int client_id = packet.ReadInt();

                if (client_id == 0)
                {
                    return;
                }

                if (clients[client_id].udp.end_point == null)
                {
                    clients[client_id].udp.Connect(client_end_point);
                    return;
                }

                if (clients[client_id].udp.end_point.ToString() == client_end_point.ToString())
                {
                    clients[client_id].udp.HandleData(packet);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving UDP data: {ex}");
        }
    }

    public static void SendUDPData(IPEndPoint client_end_point, Packet packet)
    {
        try
        {
            if (client_end_point != null)
            {
                udp_listener.BeginSend(packet.ToArray(), packet.Length(), client_end_point, null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to {client_end_point} using UDP: {ex}");
        }
    }

    private static void InitServerData()
    {
        for (int i = 1; i <= max_players; i++)
        {
            clients.Add(i, new Client(i));
        }

        packet_handler = new Dictionary<int, PacketHandler>()
            {
            //dictionary for the packets to be sent to the client, vice versa
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.player_movement, ServerHandle.PlayerMovement},
                { (int)ClientPackets.player_shoot, ServerHandle.PlayerShoot},
                { (int)ClientPackets.interpolate_pos, ServerHandle.InterpolatePos},
            };

        Debug.Log("Initialised packets...");
    }

    public static void Stop()
    {
        tcp_listener.Stop();
        udp_listener.Close();
    }
}
