using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChangeCameraButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(GameManager.singeton.GetComponent<CameraController>().ChangeCam);
    }
}