using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCamera : MonoBehaviour
{
    //obj refs
    [SerializeField] private Camera cameraComp;
    [SerializeField] private BoidScript target;
    [SerializeField] private Transform pivot;

    //vars
    private Vector3 currentSpeed;
    private float moveSpeed = 0.1f;
    //private float rotationSpeed = 1.2f;
    private float zoomSpeed = 4f;
    [SerializeField] private Vector3 targetPosDepthOffset = new Vector3 (0, 0, -5);
    private CameraMovement camMove = new CameraMovement();

    private void Start()
    {
        transform.localPosition = targetPosDepthOffset;
    }


    void Update()
    {
        if (target is not null)
        {

            camMove.DragToRotate(pivot);

            //lerp position
            Vector3 targetPos = target.gameObject.transform.position;
            pivot.position = Vector3.SmoothDamp(pivot.position, targetPos, ref currentSpeed, moveSpeed);


            float scrollAmount = Input.GetAxisRaw("Mouse ScrollWheel");

            if (scrollAmount != 0)
            {
                float newDepthVal = Mathf.Clamp(scrollAmount * zoomSpeed + targetPosDepthOffset.z, -25f, -3f);
                targetPosDepthOffset = new Vector3(0, 0, newDepthVal);
                transform.localPosition = targetPosDepthOffset;
            }

            
        }
    }

    public BoidScript getTarget() { return target; }
    public void setTarget(BoidScript target) { this.target = target; }
}
