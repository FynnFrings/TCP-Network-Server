using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;

    //set the data buffer size
    public static int dataBufferSize = 4096;

    //local host IP address
    public string ip = "127.0.0.1"; 

    //port that is used
    public int port = 26950;

    //the id of the client
    public int myId = 0;

    //TCP class
    public TCP tcp;

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
    }

    public void ConnectToServer()
    {
        //Calls the Connect() method
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
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

            //begins to read the stream of the socket
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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
                    //TODO: disconnect
                    return;
                }

                //data byte amount is set to the length of the byte
                byte[] _data = new byte[_byteLength];

                //copies the received buffer array to the data array using the length of the byte
                Array.Copy(receiveBuffer, _data, _byteLength);

                //TODO: handle Data


                //begins to read the stream of the socket
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                //TODO: disconnect
            }
        }
    }
}
