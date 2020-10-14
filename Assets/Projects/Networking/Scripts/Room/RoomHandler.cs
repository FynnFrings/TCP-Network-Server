using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public static RoomHandler instance;

    public GameObject roomPrefab;
    public GameObject roomContainer;

    public List<GameObject> roomList;

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

    public void AddRoom(int _roomId, int _maxPlayers, int _currentPlayers, string _roomName, bool _isPrivate)
    {
        //Instantiates Room Prefab
        GameObject _roomPrefab = Instantiate(roomPrefab, transform.position, Quaternion.identity, roomContainer.transform);
        RoomPrefab roomPrefabScript = _roomPrefab.GetComponent<RoomPrefab>();

        //Values
        roomPrefabScript.roomId = _roomId;
        roomPrefabScript.roomName = _roomName;

        roomPrefabScript.currentPlayers = _currentPlayers;
        roomPrefabScript.maxPlayers = _maxPlayers;
        roomPrefabScript.isPrivate = _isPrivate;

        //Adds to List
        roomList.Add(_roomPrefab);


        Debug.Log($"Room Created:\n" + 
            $"Room Id: {_roomId}\n" + 
            $"Max Players: {_maxPlayers}\n" +
            $"Current Players: {_currentPlayers}\n" +
            $"RoomName: {_roomName}"
            );
    } 

    public void DeleteAllRooms()
    {
        //Destroy all GameObjects
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i].gameObject);
        }

        //Clear list
        roomList.Clear();

        Debug.Log("All rooms deleted.");
    }
}
