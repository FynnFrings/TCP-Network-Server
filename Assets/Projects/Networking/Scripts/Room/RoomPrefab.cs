using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomPrefab : MonoBehaviour
{
    //Values
    public string roomName;

    public int currentPlayers;
    public int maxPlayers;

    //Assignments
    public int roomId; 

    public TMP_Text serverName_Text;
    public TMP_Text playerCount_Text;
    public TMP_Text ping_Text;

    public bool isPrivate;

    public Button joinButton;
    public TMP_Text joinButtonText;

    public Image privateIcon;

    // Start is called before the first frame update
    void Start()
    {
        //Sets the ping
        ping_Text.text = $" <color=#{UIManager.instance.colorHex}>{Client.instance.latency}</color>";

        //Sets name of Server
        serverName_Text.text = roomName;

        //Sets player of server
        playerCount_Text.text = currentPlayers + " / " + maxPlayers;

        //Sets Join Button
        if (currentPlayers >= maxPlayers)
        {
            joinButton.interactable = false;

            joinButtonText.text = "Full";
            joinButtonText.color = new Color32 (255, 255, 255, 200);
        }

        if(isPrivate)
        {
            privateIcon.gameObject.SetActive(true);
        }
        else
        {
            privateIcon.gameObject.SetActive(false);
        }
    }

    public void JoinRoom()
    {
        ClientSend.JoinRoom(roomId);

        //ClientSend.RequestRoomList();
    }
}
