using UnityEngine;

public class Accelerate : ButtonPressed
{
    void Start()
    {
        buttonPressed = false;
        onPressed = OnPressed;
        onEndPressing = OnEndPressing;
    }

    private void OnEndPressing()
    {
        InputManager.instance.isToUp = false;
    }

    private void OnPressed()
    {
        InputManager.instance.isToUp = true;
    }
}