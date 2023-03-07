// #define UNITY_ANDROID

using System;
using UnityEngine;

/// <summary>
/// Script which manages the input of game
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private float swipeThreshold = 0.15f; //Threshold to conform swipe
    private Vector2 startPos, endPos; //vector2 to decide swipe dircetion
    private Vector2 difference; //get the difference between startPos and endPos
    private SwipeType swipe = SwipeType.NONE; //save swipeType
    private float swipeTimeLimit = 0.25f; //TimeLimit to conform swipe
    private float startTime, endTime; //times to get difference

    public Action<SwipeType> swipeCallback; //SwipeType event trigger
    public Action<AccelerationType> acceleration;

    public GameObject[] mobileControls;

    public bool isToUp;
    public bool isToDown;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        bool mobile = false;

#if UNITY_ANDROID
        mobile = true;
#endif

        foreach (GameObject control in mobileControls)
        {
            control.SetActive(mobile);
        }
    }

    private void Update()
    {
#if UNITY_ANDROID
        /*if (Input.GetMouseButtonDown(0))                    //on mouse down
        {
            startPos = endPos = Input.mousePosition;        //set the startPos and endPos
            startTime = endTime = Time.time;                //set the startTime and endTime
        }

        if (Input.GetMouseButtonUp(0))                      //on mouse up
        {
            endPos = Input.mousePosition;                   //set the endPos
            endTime = Time.time;                            //set the endTime
            if (endTime - startTime <= swipeTimeLimit)      //check time difference
            {
                DetectSwipe();                              //if less tha limit then call method
            }
        }*/
        if (acceleration != null)
        {
            AccelerationType accelerationType = AccelerationType.None;
            if (isToUp)
                accelerationType = AccelerationType.Accelerate;
            else if (isToDown)
                accelerationType = AccelerationType.Decelerate;
            else
                accelerationType = AccelerationType.None;
            acceleration(accelerationType);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyUp(KeyCode.A))
        {
            swipeCallback(SwipeType.LEFT);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            swipeCallback(SwipeType.RIGHT);
        }

        if (acceleration != null)
        {
            AccelerationType accelerationType = AccelerationType.None;
            if (Input.GetKey(KeyCode.W))
            {
                accelerationType = AccelerationType.Accelerate;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                accelerationType = AccelerationType.Decelerate;
            }

            acceleration(accelerationType);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.singeton.GetComponent<CameraController>().ChangeCam();
        }
#endif
    }


    public void ToLeft()
    {
        Swipe(SwipeType.LEFT);
    }

    public void ToRight()
    {
        Swipe(SwipeType.RIGHT);
    }

    public void Swipe(SwipeType type)
    {
        swipeCallback(type);
    }

    public void Accelerate(AccelerationType type)
    {
        acceleration(type);
    }

    private void DetectSwipe() //decide swipe direction and swipe
    {
        swipe = SwipeType.NONE;
        difference = endPos - startPos; //get the difference
        if (difference.magnitude > swipeThreshold * Screen.width) //check if magnitude is more than Threshold
        {
            if (difference.x > 0) //right swipe
            {
                swipe = SwipeType.RIGHT;
            }
            else if (difference.x < 0) //left swipe
            {
                swipe = SwipeType.LEFT;
            }
        }

        swipeCallback(swipe); //call the event
    }
}