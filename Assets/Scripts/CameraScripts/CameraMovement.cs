using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement
{


    private float rotationSpeed = 500f;

    private bool isDragValid = true;
    private bool hasDragStarted = false;


    public void DragToRotate(Transform rotateObj)
    {
        if (Input.GetMouseButton(1))
        {
            if (hasDragStarted == false)
            {
                OnDragStart();
                return;
            }

            if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && isDragValid)
            {
                Vector3 targetRot = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * rotationSpeed * Time.deltaTime;
                rotateObj.Rotate(targetRot);
                rotateObj.rotation = Quaternion.Euler(new Vector3(rotateObj.rotation.eulerAngles.x, rotateObj.rotation.eulerAngles.y, 0)); // lock rotation of z axis
            }
        }
        else
        {
            if(hasDragStarted == true)
            {
                OnDragEnd();
            }
        }
    }

    private void OnDragStart()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isDragValid = false;
        }
        hasDragStarted = true;
    }

    public void OnDragEnd()
    {
        isDragValid = true;
        hasDragStarted = false;
    }

    public void MoveWithKeys(Transform transform, float moveSpeed)
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.position += transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            transform.position += transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        }
    }
}
