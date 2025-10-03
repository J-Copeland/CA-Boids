using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject staticCamera;
    [SerializeField] private GameObject actionCamera;

    private Camera[] cameraArr;
    private Camera activeCamera;

    private void Awake()
    {
        cameraArr = new Camera[] { staticCamera.GetComponent<Camera>(), actionCamera.GetComponent<Camera>() };
        activeCamera = cameraArr[0];
    }

    public void ToggleCamera()
    {
        activeCamera.depth = 0;
        activeCamera.gameObject.SetActive(false);

        int currentIndex = Array.IndexOf(cameraArr, activeCamera);
        activeCamera = cameraArr[(currentIndex + 1) % 2];

        activeCamera.gameObject.SetActive(true);
        activeCamera.depth = 1;
    }

    public Camera GetActiveCamera() { return activeCamera; }
    public void SetActiveCamera(Camera activeCamera) { this.activeCamera = activeCamera; }


    public GameObject GetStaticCamera() { return staticCamera; }
    public GameObject GetActionCamera() { return actionCamera; }
}
