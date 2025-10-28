using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionWeightingTestScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector3 rotation1;
    [SerializeField] Vector3 rotation2;
    [SerializeField] float weight1;
    [SerializeField] float weight2;
    [SerializeField] Quaternion quat1 = Quaternion.identity;
    [SerializeField] Quaternion quat2 = Quaternion.identity;
    
    

    private Vector3 axis1, axis2, axisTotal;
    private float angle1, angle2, angleTotal;
    [SerializeField] private Quaternion result;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        quat1 = Quaternion.Euler(rotation1);
        quat2 = Quaternion.Euler(rotation2);


        CalculateQuaternion();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, result, Time.deltaTime * speed);
    }

    private void CalculateQuaternion()
    {
        quat1.ToAngleAxis(out angle1, out axis1);
        quat2.ToAngleAxis(out angle2, out axis2);

        angle1 *= weight1;
        angle2 *= weight2;

        angleTotal = angle1 + angle2;
        axisTotal = axis1 + axis2;

        result = Quaternion.AngleAxis(angleTotal/2, axisTotal/2);
    }
}
