using UnityEngine;

/// <summary>
/// Script which keep track of game status and selected car 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager singeton;

    [HideInInspector] public GameStatus gameStatus = GameStatus.NONE;
    [HideInInspector] public int currentCarIndex = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (singeton == null)
        {
            singeton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            CameraController controller = singeton.GetComponent<CameraController>();
            controller.cameras = GetComponent<CameraController>().cameras;
            controller.ApplyCamera();
            Destroy(gameObject);
        }
    }
}