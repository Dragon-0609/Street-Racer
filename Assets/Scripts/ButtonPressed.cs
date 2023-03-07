using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;

    public Action onStartedPressing = () => { };
    public Action onPressed = () => { };
    public Action onEndPressing = () => { };

    public void Start()
    {
        buttonPressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        onStartedPressing();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
        onEndPressing();
    }

    void Update()
    {
        if (buttonPressed)
        {
            onPressed();
        }
    }
}