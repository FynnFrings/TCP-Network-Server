using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prolog : MonoBehaviour
{
    public List<GameObject> prologTextList;
    public int currentPrologText;

    public GameObject pressButtonText;

    public bool mainMenu;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && pressButtonText.GetComponent<FlyInAnimation>().animationDone)
        {
            if (currentPrologText < prologTextList.Count)
            {
                if(currentPrologText == prologTextList.Count - 1)
                {
                    pressButtonText.SetActive(false);
                }
                prologTextList[currentPrologText].SetActive(true);

                currentPrologText++;
            }
            else
            {

            }
        }
    }
}
