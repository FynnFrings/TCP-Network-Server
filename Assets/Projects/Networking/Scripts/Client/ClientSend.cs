using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;

public class ClientSend : MonoBehaviour
{
    //Sends TCP Data
    private static void SendTCPData(Packet _packet)
    {
        //Inserts the length of the packets content at the start of the buffer
        _packet.WriteLength();

        //Sends Packet
        Client.instance.tcp.SendData(_packet);
    }

    //Sends UDP Data
    private static void SendUDPData(Packet _packet)
    {
        //Inserts the length of the packets content at the start of the buffer
        _packet.WriteLength();

        //Sends Packet
        Client.instance.udp.SendData(_packet);
    }

    #region PACKETS

    //Welcome Package Received
    public static void WelcomeReceived()
    {
        //Creates new Packet
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            //Writes the ID of Client
            _packet.Write(Client.instance.myId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void SetUsername(string _username)
    {
        //Creates new Packet
        using (Packet _packet = new Packet((int)ClientPackets.setUsername))
        {
            //Writes the username of Client
            _packet.Write(_username);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void CreateRoom(int _maxPlayers, string _roomName, bool _isPrivate)
    {
        using(Packet _packet = new Packet((int)ClientPackets.createRoom))
        {

            _packet.Write(_maxPlayers);
            _packet.Write(_roomName);
            _packet.Write(_isPrivate);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void JoinRoom(int _roomId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.joinRoom))
        {

            _packet.Write(_roomId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }
    
    public static void LeaveRoom(int _roomId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.leaveRoom))
        {

            _packet.Write(_roomId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void RequestRoomList()
    {
        using (Packet _packet = new Packet((int)ClientPackets.requestRoomList))
        {
            //Writes the ID of Client
            _packet.Write(Client.instance.myId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void JoinedSuccessfully()
    {
        using (Packet _packet = new Packet ((int)ClientPackets.joinedSuccessfully))
        {
            //Writes the ID of Client
            _packet.Write(Client.instance.myId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void JoinRandomMatch()
    {
        using (Packet _packet = new Packet((int)ClientPackets.joinRandomMatch))
        {
            //Writes the ID of Client
            _packet.Write(Client.instance.myId);

            //Sends Packet
            SendTCPData(_packet);
        }
    }

    public static void Ping()
    {
        //Creates new Packet
        using (Packet _packet = new Packet((int)ClientPackets.ping))
        {
            //Writes the current Time of Client
            _packet.Write(System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.fff"));
            _packet.Write(System.DateTime.UtcNow.Millisecond);
            _packet.Write(System.DateTime.UtcNow.Ticks);

            //Sends Packet
            SendUDPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            //Writes the username of Client
            _packet.Write(_inputs.Length);

            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }

            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            //Sends Packet
            SendUDPData(_packet);
        }
    }

    public static void PlayerMousePosition(Vector3 _mousePosition)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMousePosition))
        {
            _packet.Write(_mousePosition);

            SendUDPData(_packet);
        }
    }

    #endregion
}
