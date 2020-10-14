using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

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

    public void SpawnPlayer(int _id, string _username, int _roomId, Vector3 _position, Quaternion _rotation)
    {
        //Debug.Log("Contains: " + players.ContainsKey(_id) + " / " + _username);

        GameObject _player;

        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        _player.GetComponent<PlayerManager>().roomId = _roomId;

        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void DeleteAllPlayers()
    {
        foreach (var key in players.Keys)
        {
            Destroy(players[key].gameObject);
        }

        players.Clear();
    }
}
