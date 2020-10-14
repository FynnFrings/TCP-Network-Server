using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject serverMenu;
    public GameObject startMenu;
    public TMP_InputField usernameField;
    public Button connectButton;

    public GameObject connectionErrorPopUp;
    public GameObject JoinRandomRoomErrorPopUp;
    public GameObject lostConnectionErrorPopUp;

    public TMP_InputField serverId;

    public GameObject roomMenu;
    public GameObject connectToServerLoadGameObject;

    //Create Room
    public GameObject createRoomMenu;
    public GameObject RoomListUi;
    public TMP_InputField serverNameField;
    public Slider playerSlider;
    public Toggle privateToggle;

    public TMP_Text pingText;
    public string colorHex;

    public TMP_Text infoText;
    public float deltaTime;
    public float currentTime;

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

        Application.targetFrameRate = 50;
    }

    public void Start()
    {
        StartCoroutine(ConnectToServer());
    }

    public void Update()
    {
        //FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        infoText.text = Mathf.Ceil(fps).ToString();

        //Ping
        if(Client.instance.latency <= 50)
        {
            colorHex = "40AD35";
        }
        else if (Client.instance.latency > 50 && Client.instance.latency < 125)
        {
            colorHex = "D1AD35";
        }
        else
        {
            colorHex = "E14D35";
        }

        //Change Text
        pingText.text = $"Ping: <color=#{colorHex}>{Client.instance.latency}</color>";

        #region LOADINGSCREEN

        if (connectToServerLoadGameObject.activeSelf)
        {
            if (currentTime > 7)
            {
                if (!connectionErrorPopUp.activeSelf)
                {
                    connectionErrorPopUp.SetActive(true);

                    Debug.Log($"Connection Timed Out (not connected to Server)");

                    StartCoroutine(ConnectToServer());
                }
            }
            else
            {
                currentTime += Time.deltaTime;

                StartCoroutine(ConnectToServerLoadingScreen());
            }
        }

        #endregion
    }

    public IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        if (!Client.instance.isConnected)
        {
            currentTime = 0;

            connectToServerLoadGameObject.SetActive(true);

            //calls ConnectToServer() Method of the client
            Client.instance.ConnectToServer();
        }
    }

    IEnumerator ConnectToServerLoadingScreen()
    {
        if (!Client.instance.isConnected)
        {

        }
        else
        {
            connectionErrorPopUp.SetActive(false);

            yield return new WaitForSeconds(1.5f);


            startMenu.SetActive(true);
            connectToServerLoadGameObject.SetActive(false);
        }
    }

    public void SetUsername()
    {
        if (!string.IsNullOrWhiteSpace(usernameField.text))
        {
            Debug.Log($"The name of Player {Client.instance.myId} is set to: {usernameField.text}.");
        }
        else
        {
            usernameField.text = "Player" + Client.instance.myId;

            Debug.Log($"The name of Player {Client.instance.myId} is not assigned properly (IsNullOrWhiteSpace)! Default name assigned ({usernameField.text}).");
        }

        string _username = usernameField.text;

        ClientSend.SetUsername(_username);

        usernameField.interactable = false;
        connectButton.interactable = false;

    }

    public void CreateRoom()
    {
        string _serverName;

        if (!createRoomMenu.activeSelf)
        {
            createRoomMenu.SetActive(true);
            RoomListUi.SetActive(false);

            serverNameField.placeholder.GetComponent<TextMeshProUGUI>().text = $"{usernameField.text}'s Room";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(serverNameField.text))
            {
                _serverName = serverNameField.text;
            }
            else
            {
                _serverName = $"{usernameField.text}'s Room";
            }

            foreach (var key in GameManager.players.Keys.ToList())
            {
                Destroy(GameManager.players[key].gameObject);
                GameManager.players.Remove(key);    
            }

            ClientSend.CreateRoom((int)playerSlider.value, _serverName, privateToggle.isOn);

            createRoomMenu.SetActive(false);
            RoomListUi.SetActive(true);
            serverNameField.text = "";

            RequestRoomList();
        }
    }

    public void JoinRoom()
    {
        ClientSend.JoinRoom(int.Parse(serverId.text));
    }

    public void LeaveRoom()
    {
        Debug.Log("left room: " + GameObject.FindWithTag("LocalPlayer").GetComponent<PlayerManager>().roomId);
        ClientSend.LeaveRoom(GameObject.FindWithTag("LocalPlayer").GetComponent<PlayerManager>().roomId);
    }

    public void JoinRandomMatch()
    {
        ClientSend.JoinRandomMatch();
    }

    public void NoAvailableRoomFound()
    {
        JoinRandomRoomErrorPopUp.SetActive(true);
    }

    public void RequestRoomList()
    {
        ClientSend.RequestRoomList();
    }

    public void DisplayGame()
    {
        serverMenu.SetActive(false);
        roomMenu.SetActive(false);
        startMenu.SetActive(false);
    }

    public void LostConnection()
    {
        lostConnectionErrorPopUp.SetActive(true);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
