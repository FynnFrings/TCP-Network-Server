using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            //Writes the username of Client
            _packet.Write(UIManager.instance.usernameField.text);

            //Sends Packet
            SendTCPData(_packet);
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

    #endregion
}
