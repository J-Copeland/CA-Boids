using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    //obj refs
    [SerializeField] private Camera cameraComp;

    //vars
    private Vector3 currentSpeed;
    private float moveSpeed = 20f;
    private CameraMovement camMove = new CameraMovement();

    private void Start()
    {
    }


    void Update()
    {
        camMove.DragToRotate(transform);
        camMove.MoveWithKeys(transform, moveSpeed);
    }


}
