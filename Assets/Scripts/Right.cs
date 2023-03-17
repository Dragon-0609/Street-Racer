using UnityEngine;

public class Right : ButtonPressed
{
    void Start()
    {
        buttonPressed = false;
        onPressed = OnPressed;
        onEndPressing = OnEndPressing;
    }

    private void OnEndPressing()
    {
        InputManager.instance.isToRight = false;
    }

    private void OnPressed()
    {
        InputManager.instance.isToRight = true;
    }
}