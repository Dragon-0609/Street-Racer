public class Decelerate : ButtonPressed
{
    void Start()
    {
        buttonPressed = false;
        onPressed = OnPressed;
        onEndPressing = OnEndPressing;
    }

    private void OnEndPressing()
    {
        InputManager.instance.isToDown = false;
    }

    private void OnPressed()
    {
        InputManager.instance.isToDown = true;
    }
}