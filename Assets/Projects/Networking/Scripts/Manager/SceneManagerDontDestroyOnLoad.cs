using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerDontDestroyOnLoad : MonoBehaviour
{
    public static bool sceneManagerExists;

    //public static Client instance;

    //Language
    [Header("Language")]
    public int language;

    // Start is called before the first frame update
    void Awake()
    {
        if (!sceneManagerExists)
        {
            sceneManagerExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Client.instance.isConnected)
        {
            ClientSend.Ping();
        }
    }
}
