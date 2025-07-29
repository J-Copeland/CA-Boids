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
    private float rotationSpeed = 1.2f;
    private float zoomSpeed = 0.2f;
    [SerializeField] private Vector3 targetPosDepthOffset = new Vector3 (0, 0, -5);


    private void Start()
    {
        transform.position = targetPosDepthOffset;
    }


    void Update()
    {
        if (target is not null)
        {
            //lerp position
            Vector3 targetPos = target.gameObject.transform.position;
            pivot.position = Vector3.SmoothDamp(pivot.position, targetPos, ref currentSpeed, moveSpeed);



            if (Input.GetMouseButton(1))
            {
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    Vector3 targetRot = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * rotationSpeed;
                    pivot.Rotate(targetRot);
                    pivot.rotation = Quaternion.Euler(new Vector3(pivot.rotation.eulerAngles.x, pivot.rotation.eulerAngles.y, 0)); // lock rotation of z axis
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                Debug.Log(Input.GetAxisRaw("Mouse ScrollWheel"));
                targetPosDepthOffset += new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed);
                targetPosDepthOffset += new Vector3(0, 0, Mathf.Clamp(Mathf.Abs(targetPosDepthOffset.z), 1f, 25f));
                transform.position = targetPosDepthOffset;
            }

            
        }
    }

    public BoidScript getTarget() { return target; }
    public void setTarget(BoidScript target) { this.target = target; }
}
