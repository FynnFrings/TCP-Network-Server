using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using MyAttributes;
using UnityEditor;

public class FlyInAnimation : MonoBehaviour
{
    #region VARIABLES

    //Object
    #region ANIMATED OBJECT: VARIABLES

    [Header("Animated Object")]

    [Tooltip("Tick only if animation is on text")]
    public bool isText;                   //animation is on text

    [Tooltip("Tick only if animation is on image")]
    public bool isImage;                  //animation is on image

    #endregion

    //Text
    #region TEXT ANIMATION: VARIABLES

    [Header("Text Animation")]

    [Tooltip("Tick if you want the text to be animated")]
    public bool textAnimation;          //If is true: the variables from beneath are visible in the Inspector!

    [ConditionalField("textAnimation")]
    [Tooltip("The speed each character takes to instantiate")]
    public float typingSpeed = 0.05f;   //The speed each character takes to instantiate

    [ConditionalField("textAnimation")]
    [Tooltip("The last letter instantiated is bold")]
    public bool startsBold;             //The last letter instantiated is bold

    [ConditionalField("textAnimation")]
    [Tooltip("The last letter instantiated is italic")]
    public bool startsitalic;           //The last letter instantiated is italic

    [ConditionalField("textAnimation")]
    [Tooltip("The last letter instantiated is underlined")]
    public bool startsUnderlined;       //The last letter instantiated is underlined

    [ConditionalField("textAnimation")]
    [Tooltip("Is true when the text animation is done")]
    [ReadOnly]
    public bool textAnimationDone;          //Is true when the text animation is done

    #endregion

    //From Side
    #region FLY IN FROM OUTSIDE OF SCREEN: VARIABLES

    [Header("Fly in from outside of screen")]

    [Tooltip("Tick if you want the GameObject to Fly in from anywhere outside the screen")] 
    public bool fromSide;               //If is true: the booleans from beneath are visible in the Inspector!

    [ConditionalField("fromSide")] 
    [Tooltip("Fly in from TOP of Screen")] 
    [Space(5)]
    public bool fromTop;                //Fly in from top of Screen

    [ConditionalField("fromSide")] 
    [Tooltip("Fly in from BOTTOM of Screen")] 
    [Space(-2)]
    public bool fromBottom;             //Fly in from bottom of Screen

    [ConditionalField("fromSide")] 
    [Tooltip("Fly in from RIGHT of Screen")] 
    [Space(-2)]
    public bool fromRight;              //Fly in from right of Screen

    [ConditionalField("fromSide")] 
    [Tooltip("Fly in from LEFT of Screen")] 
    [Space(-2)]
    public bool fromLeft;               //Fly in from left of Screen

    #endregion

    //From current Position
    #region FLY IN FROM CURRENT POSITION: VARIABLES 

    [Header("Fly in from current position")]

    [Tooltip("Tick if you want the GameObject to Fly in from the current position")]
    public bool fromCurrentPosition;    //If is true: the booleans from beneath are visible in the Inspector!

    [ConditionalField("fromCurrentPosition")] 
    [Tooltip("Opacity of TEXT or IMAGE increases from Zero to Current")] 
    [Space(5)]
    public bool fadeIn;                 //Opacity of text or image increases from Zero to Current

    [ConditionalField("fromCurrentPosition")] 
    [Tooltip("Scale increases from Zero to Current")]
    public bool zoomIn;                 //Scale increases from Zero to Current

    #endregion

    //Animation Time
    #region ANIMATION TIME: VARIABLES

    [Header("Animation Time")]

    [Tooltip("Time in seconds the animation takes to finish")]
    public float animationTime = 0.75f; //How long the Animation lasts in Seconds

    [Tooltip("Time in seconds the animation takes to start")]
    public float waitTime = 0.05f;      //How long to wait till Animation starts in Seconds (AFTER first frame of Animation)

    [Tooltip("Shows the current Time of the animation")]
    [ReadOnly]
    public float currentTime = 0f;      //Shows the current Time of the animation

    [Tooltip("Is true when the animation is done")]
    [ReadOnly]
    public bool animationDone;          //Is true when the animation is done

    #endregion

    //Debug
    #region DEBUG: VARIABLES

    [Header("Debug")]

    [Tooltip("If is true: private variables are visible in the Inspector!")]
    public bool showPrivateVariables;   //If is true: the variables from beneath are visible in the Inspector!

    [ConditionalField("showPrivateVariables")]
    [ReadOnly]
    [SerializeField]
    private Color startColor;           //Color at start

    [ConditionalField("showPrivateVariables")]
    [ReadOnly]
    [SerializeField]
    private Color color;                //new Color (alpha)

    [ConditionalField("showPrivateVariables")]
    [ReadOnly]
    [SerializeField]
    private string startText;           //text at start

    [ConditionalField("showPrivateVariables")]
    [ReadOnly]
    [SerializeField]
    private Vector3 startPos;           //Position at start

    [ConditionalField("showPrivateVariables")]
    [ReadOnly]
    [SerializeField]
    private Vector3 startPosScale;      //Scale at start

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        startPosScale = transform.localScale;

        animationDone = false;
        textAnimationDone = false;

        #region CHECK BOOLEANS

        if (!fromSide)
        {
            fromTop = false;
            fromBottom = false;
            fromRight = false;
            fromLeft = false;
        }
        if(!fromCurrentPosition)
        {
            fadeIn = false;
            zoomIn = false;
        }

        #endregion

        #region IF SCRIPT IS ON TEXT OR IMAGE

        if (isText)
        {
            startText = gameObject.GetComponent<TextMeshProUGUI>().text;

            color = gameObject.GetComponent<TextMeshProUGUI>().color;
            startColor = color;
            color.a = 0f;
        }

        else if (isImage)
        {
            color = gameObject.GetComponent<Image>().color;
            startColor = color;
            color.a = 0f;
        }

        #endregion

        //Start Animation
        StartCoroutine(StartAnimation());

        if(textAnimation)
        {
            //Text specific animations
            StartCoroutine(TextAnimations());
        }
    }

    //Start Animation
    IEnumerator StartAnimation()
    {
        #region START ANIMATION

        while (currentTime <= 1.0)
        {
            currentTime += Time.deltaTime / animationTime;

            //Fly in from outside of Screen
            FlyInFromOutsideOfScreen();

            //Fly in from current Position
            FlyInFromCurrentPosition();

            #region WAIT FOR ANIMATION TO START

            //Wait until Start of Animation (AFTER first Frame)
            if (waitTime > 0)
                    {
                        yield return new WaitForSeconds(waitTime);
                        waitTime = 0f;
                    }

            #endregion

        yield return null;
        }

        #region NORMALIZE EVERYTHING

        if (isText)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = startColor;
        }
        else if (isImage)
        {
            gameObject.GetComponent<Image>().color = startColor;
        }

        transform.localPosition = startPos;

        #endregion

        animationDone = true;

        #endregion
    }

    private void FlyInFromOutsideOfScreen()
    {
        #region FLY IN FROM DIFFERENT POSITION

        //Fly in from Top
        if (fromTop)
        {
            transform.localPosition = Vector3.Lerp(new Vector3(0, Screen.height * 2, 0), startPos, Mathf.SmoothStep(0f, 1f, currentTime));
        }

        //Fly in from Bottom
        else if (fromBottom)
        {
            transform.localPosition = Vector3.Lerp(new Vector3(0, -Screen.height * 2, 0), startPos, Mathf.SmoothStep(0f, 1f, currentTime));
        }

        //Fly in from Right
        else if (fromRight)
        {
            transform.localPosition = Vector3.Lerp(new Vector3(Screen.width * 2, startPos.y, 0), startPos, Mathf.SmoothStep(0f, 1f, currentTime));
        }

        //Fly in from Left
        else if (fromLeft)
        {
            transform.localPosition = Vector3.Lerp(new Vector3(-Screen.width * 2, startPos.y, 0), startPos, Mathf.SmoothStep(0f, 1f, currentTime));
        }

        #endregion
    }

    private void FlyInFromCurrentPosition()
    {
        #region FLY IN FROM CURRENT POSITION

        //Fade In (color opacity)
        if (fadeIn)
        {
            if (isText)
            {
                gameObject.GetComponent<TextMeshProUGUI>().color = color;
                color.a = currentTime * startColor.a;
            }
            else if (isImage)
            {
                gameObject.GetComponent<Image>().color = color;
                color.a = currentTime * startColor.a;
            }
        }

        //Zoom In (Scale)
        if (zoomIn)
        {
            transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(startPosScale.x, startPosScale.y, startPosScale.z), Mathf.SmoothStep(0f, 1f, currentTime));
        }

        #endregion
    }

    IEnumerator TextAnimations()
    {
        #region TEXT ANIMATION

        gameObject.GetComponent<TextMeshProUGUI>().text = "";

        foreach (char letter in startText.ToCharArray())
        {
            //stores original text
            string text = gameObject.GetComponent<TextMeshProUGUI>().text;

            #region MODIFY TEXT ON LAST CHARACTER

            string modifystart = "";
            string modifyend = "";

            if (startsBold)
            {
                modifystart += "<b>";
                modifyend += "</b>";
            }
            if(startsitalic)
            {
                modifystart += "<i>";
                modifyend += "</i>";
            }
            if(startsUnderlined)
            {
                modifystart += "<u>";
                modifyend += "</u>";
            }

            #endregion

            gameObject.GetComponent<TextMeshProUGUI>().text += modifystart + letter + modifyend;
            yield return new WaitForSeconds(typingSpeed);

            gameObject.GetComponent<TextMeshProUGUI>().text = text;
            gameObject.GetComponent<TextMeshProUGUI>().text += letter;
        }

        textAnimationDone = true;

        #endregion
    }
}

