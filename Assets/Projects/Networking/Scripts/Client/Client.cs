using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using GameServer;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class Client : MonoBehaviour
{
    #region ATTRIBUTES

    public static Client instance;

    //set the data buffer size
    public static int dataBufferSize = 4096;

    //local host IP address
    public string ip = "192.168.2.152";

    //public host ip address
    public string publicIp = "82.82.32.197";

    //port that is used
    public int port = 26950;

    //the id of the client
    [ReadOnly] public int myId = 0;

    //TCP class
    public TCP tcp;

    //UDP class
    public UDP udp;

    //is true when connected
    [ReadOnly] public bool isConnected = false;

    //the last detected latency in ms
    [ReadOnly] public int latency;

    //Time not connected
    [ReadOnly] public float connectionLostSince = 0;

    //handles the packet
    private delegate void PacketHandler(Packet _packet);

    //Stores the packets
    private static Dictionary<int, PacketHandler> packetHandlers;

    #endregion

    private void Awake()
    {
        //If instance does not already exists
        if (instance == null)
        {
            //This is the instance now
            instance = this;
        }
        else
        {
            //Instance already exists
            Debug.Log($"Instance already exists ({instance}), destroying this new Instance!");

            //Destroy this. because it is already available
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void FixedUpdate()
    {
        ConnectionCheck();
    }

    private void OnApplicationQuit()
    {
        Disconnect();    
    }

    public void ConnectToServer()
    {
        //Initializes the Data of the Client
        InitializeClientData();

        //Calls the Connect() method
        tcp.Connect();
    }

    public class TCP
    {
        //socket of the Client
        public TcpClient socket;

        //stream of Data
        private NetworkStream stream;

        //received Data packet
        private Packet receivedData;

        //Buffer of received Packet
        private byte[] receiveBuffer;

        public void Connect()
        {
            //send/receive buffer size is assigned to the socket
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            //the receive buffer byte is a new byte with given dataBufferSize
            receiveBuffer = new byte[dataBufferSize];

            //connects to the socket
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            //stops the connection attempt
            socket.EndConnect(_result);

            //if socket is not connected
            if(!socket.Connected)
            {
                return;
            }

            //gets the stream of the socket
            stream = socket.GetStream();

            receivedData = new Packet();

            //begins to read the stream of the socket
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                //If socket is not empty
                if (socket != null)
                {
                    //Writes on stream
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending Data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                //stops to read 
                int _byteLength = stream.EndRead(_result);

                //byte probably contains no data
                if (_byteLength <= 0)
                {
                    //Disconnect
                    instance.Disconnect();

                    return;
                }

                //data byte amount is set to the length of the byte
                byte[] _data = new byte[_byteLength];

                //copies the received buffer array to the data array using the length of the byte
                Array.Copy(receiveBuffer, _data, _byteLength);

                //handle Data
                receivedData.Reset(HandleData(_data));

                //begins to read the stream of the socket
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");

                //Disconnect
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
            {
                //length of the packet
                int _packetLength = 0;

                //set the byte and data of the received Data
                receivedData.SetBytes(_data);

                //If the unread Data of the received Data is more than 4
                if (receivedData.UnreadLength() >= 4)
                {
                    //length of the packet is set to the length of received Data
                    _packetLength = receivedData.ReadInt();

                    //If packetLength is less or equal to 0
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                //While _packetLength is more than 0 & less than the length of unread data
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    //packetBytes is set to the bytes of the received Data
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);

                    //Executes on the main Thread
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        //creates a new Packet
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            //Sets _packetId
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](_packet);
                        }
                    });

                    //Sets packetLength to zero
                    _packetLength = 0;

                    //If the unread Data of the received Data is more than 4
                    if (receivedData.UnreadLength() >= 4)
                    {
                        //length of the packet is set to the length of received Data
                         _packetLength = receivedData.ReadInt();

                        //If packetLength is less or equal to 0
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                //If _packet is empty or 1
                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        //socket of UDP client
        public UdpClient socket;

        //Endpoint of UDP client
        public IPEndPoint endPoint;

        public UDP()
        {
            //Sets endpont to ip address & port
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        //Sends packet
        public void SendData(Packet _packet)
        {
            try
            {
                //insert packet ID at beginning 
                _packet.InsertInt(instance.myId);

                //socket is available
                if(socket != null)
                {
                    //Begin to send packet
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch(Exception _ex)
            {
                Debug.Log($"Error sending Data to Server via UDP: {_ex}");
            }
        }

        //Connect 
        public void Connect(int _localPort)
        {
            //socket is set to clients port
            socket = new UdpClient(_localPort);

            //connects to endpoint
            socket.Connect(endPoint);

            //begins to receive data
            socket.BeginReceive(ReceiveCallback, null);

            //Creates packet
            using (Packet _packet = new Packet())
            {
                //send packet
                SendData(_packet);
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                //stores data that has been received
                byte[] _data = socket.EndReceive(_result, ref endPoint);

                //receives new Data
                socket.BeginReceive(ReceiveCallback, null);

                //if Length of data is less than 4
                if(_data.Length < 4)
                {
                    //Disconnect
                    instance.Disconnect();

                    return;
                }

                //Handle data
                HandleData(_data);

            }
            catch
            {
                //Disconnect
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            //create new packet
            using (Packet _packet = new Packet(_data))
            {
                //set _packetLength
                int _packetLength = _packet.ReadInt();

                //set _data
                _data = _packet.ReadBytes(_packetLength);
            }

            //Execute on Main Thread
            ThreadManager.ExecuteOnMainThread(() =>
            {
                //Creates new packet
                using (Packet _packet = new Packet(_data))
                {
                    //set id of packet to received ID
                    int _packetID = _packet.ReadInt();

                    //adds to packethandlers
                    packetHandlers[_packetID](_packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        //Data is stored in Dictionary
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            //Welcome Packet
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.roomList, ClientHandle.RoomList },
            { (int)ServerPackets.spawnPLayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.spawnPlayersToRoom, ClientHandle.SpawnPlayersToRoom },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.PlayerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.playerJoinedRoom, ClientHandle.PlayerJoinedRoom },
            { (int)ServerPackets.noAvailableRoomFound, ClientHandle.NoAvailableRoomFound },
            { (int)ServerPackets.disconnectAllPlayersFromRoom, ClientHandle.DisconnectAllPlayersFromRoom },
            { (int)ServerPackets.pingReceived, ClientHandle.PingReceived },
        };
        Debug.Log("Initialized Packets.");
    }

    public void ConnectionCheck()
    {
        if (isConnected)
        {
            connectionLostSince += Time.deltaTime;

            if (connectionLostSince > 5)
            {
                Disconnect();
                UIManager.instance.LostConnection();

                Debug.Log($"Connection Timed Out ({connectionLostSince}seconds)");
            }
        }
    }

    private void Disconnect()
    {
        if(isConnected)
        {
            isConnected = false;

            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log($"Disconnected from Server.");
        }
    }
}
