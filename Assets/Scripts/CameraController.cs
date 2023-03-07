using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject[] cameras;
    private int camera = 0;

    public void ChangeCam()
    {
        camera++;
        if (camera >= cameras.Length)
            camera = 0;

        ApplyCamera();
    }

    public void ApplyCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool bcam = camera == i;
            string tag = bcam ? "MainCamera" : "Untagged";
            cameras[i].SetActive(bcam);
            cameras[i].tag = tag;
        }
    }
}