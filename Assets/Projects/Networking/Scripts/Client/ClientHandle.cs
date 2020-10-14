using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    //Handles the Welcome Packet
    public static void Welcome(Packet _packet)
    {
        //sets message to packets message
        string _msg = _packet.ReadString();

        //sets ID to packets ID
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from Server: {_msg}");

        //Sets Client ID to packet ID
        Client.instance.myId = _myId;

        //Send welcome received message
        ClientSend.WelcomeReceived();

        Client.instance.isConnected = true;

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        //Remove all old Players
        GameManager.instance.DeleteAllPlayers();

        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        int _roomId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        Debug.Log("id: " + _id);
        GameManager.instance.SpawnPlayer(_id, _username, _roomId, _position, _rotation);
    }

    public static void SpawnPlayersToRoom(Packet _packet)
    {
        //Remove all old Players
        GameManager.instance.DeleteAllPlayers();

        int _playerCount = _packet.ReadInt();

        Debug.Log("player count: " + _playerCount);

        for (int i = 0; i < _playerCount; i++)
        {
            int _id = _packet.ReadInt();
            string _username = _packet.ReadString();
            int _roomId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            Debug.Log("id: " + _id);

            GameManager.instance.SpawnPlayer(_id, _username, _roomId, _position, _rotation);
        }
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if(GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].transform.position = _position;
        }
        else
        {
            Debug.Log($"Key not found: {_id}");
        }
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        if(GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].transform.rotation = _rotation;
        }
        else
        {
            Debug.Log($"Key not found: {_id}");
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        if (GameManager.players.ContainsKey(_id))
        {
            Destroy(GameManager.players[_id].gameObject);
            GameManager.players.Remove(_id);

            Debug.Log($"Player {_id} disconnected");
        }
    }

    public static void DisconnectAllPlayersFromRoom(Packet _packet)
    {
        int _playerCount = _packet.ReadInt();

        for (int i = 0; i < _playerCount; i++)
        {
            int _playerKey = _packet.ReadInt();

            if(GameManager.players.ContainsKey(_playerKey))
            {
                Destroy(GameManager.players[_playerKey].gameObject);
                GameManager.players.Remove(_playerKey);

                Debug.Log($"Player {_playerKey} disconnected");
            }
        }
    }

    public static void RoomList(Packet _packet)
    {
        int _roomCount = _packet.ReadInt();

        RoomHandler.instance.DeleteAllRooms();

        for (int i = 0; i < _roomCount; i++)
        {
            int _roomId = _packet.ReadInt();
            int _maxPlayers = _packet.ReadInt();
            int _currentPlayers = _packet.ReadInt();
            string _roomName = _packet.ReadString();
            bool _isPrivate = _packet.ReadBool();

            RoomHandler.instance.AddRoom(_roomId, _maxPlayers, _currentPlayers, _roomName, _isPrivate);
        }
    }

    public static void PlayerJoinedRoom(Packet _packet)
    {
        int _roomId = _packet.ReadInt();

        Debug.Log($"Player succesfully joined room: Id {_roomId}");

        UIManager.instance.DisplayGame();

        ClientSend.JoinedSuccessfully();
    }

    public static void NoAvailableRoomFound(Packet _packet)
    {
        UIManager.instance.NoAvailableRoomFound();

        Debug.Log("No available room found");
    }

    public static void PingReceived(Packet _packet)
    {
        string receivedTime = _packet.ReadString();
        int receivedTime_ms = _packet.ReadInt();
        int receivedTime_ticks = _packet.ReadInt();

        System.DateTime receivedDate = System.DateTime.ParseExact(receivedTime, "dd/MM/yyyy HH:mm:ss.fff", null);

        System.DateTime currentTime = System.DateTime.UtcNow;

        System.TimeSpan ts = currentTime.Subtract(receivedDate);

        //total milliseconds difference between two datetime object
        int milliseconds = (int)ts.TotalMilliseconds;

        Client.instance.latency = milliseconds;
        Client.instance.connectionLostSince = 0;
    }
}
