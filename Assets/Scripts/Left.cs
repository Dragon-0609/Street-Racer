using UnityEngine;

public class Left : ButtonPressed
{
    void Start()
    {
        buttonPressed = false;
        onPressed = OnPressed;
        onEndPressing = OnEndPressing;
    }

    private void OnEndPressing()
    {
        InputManager.instance.isToLeft = false;
    }

    private void OnPressed()
    {
        InputManager.instance.isToLeft = true;
    }
}