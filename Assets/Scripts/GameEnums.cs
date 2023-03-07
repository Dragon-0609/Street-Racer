using System;

/// <summary>
/// File which holds enums used in game
/// </summary>
[Serializable]
public enum SwipeType
{
    NONE,
    LEFT,
    RIGHT
}

public enum GameStatus
{
    NONE,
    PLAYING,
    FAILED
}

public enum AccelerationType
{
    Accelerate,
    Decelerate,
    None
}