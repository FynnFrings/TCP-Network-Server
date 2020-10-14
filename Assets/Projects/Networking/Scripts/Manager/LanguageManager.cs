using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    SceneManagerDontDestroyOnLoad scenemanager;

    private TMP_Text text;
    public string german;
    public string english;

    // Start is called before the first frame update
    void Start()
    {
        scenemanager = GameObject.FindWithTag("SceneManagerDontDestroyOnLoad").GetComponent<SceneManagerDontDestroyOnLoad>();

        text = gameObject.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        SetTextToLanguage();
    }

    public void SetTextToLanguage()
    {
        if (scenemanager.language == 0)
        {
            text.text = english;
        }
        else if (scenemanager.language == 1)
        {
            text.text = german;
        }
    }
}
