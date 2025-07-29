using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidScript : MonoBehaviour
{

    // Obj vars
    [SerializeField] private GameObject triContainer;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private int debugMode = 0;
    [SerializeField] private BoidScript[] boidArr;

    // Boid vars
    [SerializeField] private float fov = 2.25f;
    [SerializeField] private float sightDist = 5f;
    [SerializeField] private float separationDist = 2;
    [SerializeField] private float separationSpeed = 1; //as a val [0-1]
    [SerializeField] private float alignmentDist = 5;
    [SerializeField] private float alignmentSpeed = 1;
    [SerializeField] private float cohesionDist = 10;
    [SerializeField] private float cohesionSpeed = 1;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float movementSpeed = 2;

    [SerializeField] private bool canMove = true; // FOR TESTING -- DELETE AFTERWARDS

    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        ConfigureLineRenderer();
    }

    void Update()
    {

        // call function to calculate target direction & unpack return vals
        var boidInfo = CalculateTargetRotation();
        Quaternion targetRotation = boidInfo.Item1;
        bool boidsScanned = boidInfo.Item2;
        TargetInfo targetInfo = boidInfo.Item3;


        // if line renderer should be active - call function to draw to it
        if (debugMode != 0)
        DrawToLineRenderer(targetRotation, boidsScanned, targetInfo);


        if (canMove)   // IF STATEMENT FOR TESTING -- DELETE AFTERWARDS
        {
            // only change direction if boids have been scanned
            if(boidsScanned)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * Time.deltaTime * movementSpeed;
        }
        
    }

    public (Quaternion, bool, TargetInfo) CalculateTargetRotation()
    {
        bool boidsScanned = false;

        Vector3 separationTargetVector = new Vector3();
        int separationTargetCount = 0;
        Quaternion separationTargetDir = Quaternion.identity;

        Vector3 alignmentTargetVector = new Vector3(); //was alignmentTargetRotation
        int alignmentTargetCount = 0;
        Quaternion alignmentTargetDir = Quaternion.identity;

        Vector3 cohesionTargetVector = new Vector3();
        int cohesionTargetCount = 0;
        Quaternion cohesionTargetDir = Quaternion.identity;

        float targetAngle = 0f;
        int activeRuleCount = 0;
        Vector3 targetAxis = new Vector3(); ;
        Quaternion targetRotation;


        //logic loop
        foreach (BoidScript boid in boidArr)
        {
            Transform targetTransform = boid.gameObject.GetComponent<Transform>();
            float distance = (targetTransform.position - transform.position).magnitude;

            if (distance < separationDist)
            {
                separationTargetCount++;
                separationTargetVector += targetTransform.localPosition;
            }

            if (distance < alignmentDist)
            {
                alignmentTargetCount++;
                alignmentTargetVector += targetTransform.forward;
            }

            if (distance < cohesionDist)
            {
                cohesionTargetCount++;
                cohesionTargetVector += targetTransform.localPosition;
            }


        }

        //inverse quaternion to face away, creates a quaternion that looks towards the position
        if (separationTargetCount != 0)
        {
            separationTargetVector /= separationTargetCount;

            Vector3 posDiff = (separationTargetVector - transform.position).normalized;
            if (posDiff != Vector3.zero)
            {
                Quaternion pointToTarget = Quaternion.LookRotation(posDiff * -1);
                separationTargetDir = Quaternion.Slerp(transform.rotation, pointToTarget, separationSpeed);

                separationTargetDir.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle != 0)
                {
                    targetAngle += angle;
                    targetAxis += axis;
                }

                activeRuleCount++;
            }
            boidsScanned = true;

            
        }

        //changes sum of all alignment directions to average
        if (alignmentTargetCount != 0)
        {
            alignmentTargetVector /= alignmentTargetCount;

            Quaternion pointToTarget = Quaternion.LookRotation(alignmentTargetVector);
            alignmentTargetDir = Quaternion.Slerp(transform.rotation, pointToTarget, alignmentSpeed);

            alignmentTargetDir.ToAngleAxis(out float angle, out Vector3 axis);
            if(angle != 0)
            {
                targetAngle += angle;
                targetAxis += axis;
            }
            

            activeRuleCount++;

            boidsScanned = true;
        }


        //changes sum of all boids positions to average, creates a quaternion that looks towards the position
        if (cohesionTargetCount != 0)
        {
            cohesionTargetVector /= cohesionTargetCount;
            Vector3 posDiff = (cohesionTargetVector - transform.position).normalized;
            if(posDiff != Vector3.zero)
            {
                Quaternion pointToTarget = Quaternion.LookRotation(posDiff);
                cohesionTargetDir = Quaternion.Slerp(transform.rotation, pointToTarget, cohesionSpeed);

                cohesionTargetDir.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle != 0)
                {
                    targetAngle += angle;
                    targetAxis += axis;
                }

                activeRuleCount++;
            }
            boidsScanned = true;
        }

        // set var for debug mode 3
        TargetInfo targetInfo = null;
        if (debugMode == 3)
            targetInfo = new TargetInfo(separationTargetDir, alignmentTargetDir, cohesionTargetDir, separationTargetCount, alignmentTargetCount, cohesionTargetCount);

        // combine vectors from all behaviours
        targetAxis.Normalize();
        targetAngle /= activeRuleCount;

        targetRotation = Quaternion.AngleAxis(targetAngle, targetAxis);

        return (targetRotation, boidsScanned, targetInfo);
    }


    public (Quaternion, float, Vector3) CalculateTargetForRule(Vector3 lookDirection, float speed)
    {
        Quaternion pointToTarget = Quaternion.LookRotation(lookDirection);
        Quaternion targetDirection = Quaternion.Slerp(transform.rotation, pointToTarget, speed);

        targetDirection.ToAngleAxis(out float angle, out Vector3 axis);
        return (targetDirection, angle, axis);
    }


    public void DrawToLineRenderer(Quaternion targetRotation, bool boidsScanned, TargetInfo targetInfo)
    {
        //OPTION 1  - SHOW TARGET DIRECTION
        if(debugMode == 1)
        {
            if (boidsScanned)
            {
                //targetRotation.ToAngleAxis(out float angle, out Vector3 axis);
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position + (targetRotation * Vector3.forward) * 2);
                //lr.SetPosition(0, transform.position - axis * 2);
                //lr.SetPosition(1, transform.position + axis * 2);
            }
            else
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position + (transform.forward) * 2);
            }
        }

        // OPTION 2  - SHOW BOID FOV
        else if (debugMode == 2)
        {
            Vector3 straightDir = transform.position + transform.forward * sightDist;
            float radius = (Mathf.Tan(fov * Mathf.PI / 360) * sightDist);

            Vector3 xPos = straightDir + transform.up * radius;
            Vector3 xNeg = straightDir - transform.up * radius;
            Vector3 yPos = straightDir + transform.right * radius;
            Vector3 yNeg = straightDir - transform.right * radius;
            lr.SetPositions(new Vector3[] { transform.position, transform.position + (transform.forward * 2), transform.position, xPos, transform.position, xNeg, transform.position, yPos, transform.position, yNeg });
        }

        // OPTION 3 - SHOW SPLIT TARGET DIRECTIONS
        else if (debugMode == 3)
        {

            lr.colorGradient = CalculateTargetLinesGradient(targetInfo.sc>0, targetInfo.ac>0, targetInfo.cc>0);

            if (targetInfo.sc > 0) // separation
            {
                lr.SetPosition(0, transform.position + (targetInfo.sd * Vector3.forward) * separationDist);
                lr.SetPosition(1, transform.position);
            }
            else { 
                lr.SetPosition(0, transform.position); 
                lr.SetPosition(1, transform.position); 
            }

            // color-gradient separation lines
            lr.SetPosition(2, transform.position + transform.forward * 0.25f);
            lr.SetPosition(3, transform.position);

            if (targetInfo.ac > 0) // alignment
            {
                lr.SetPosition(4, transform.position + (targetInfo.ad * Vector3.forward) * alignmentDist);
                lr.SetPosition(5, transform.position);
            }
            else { 
                lr.SetPosition(4, transform.position);
                lr.SetPosition(5, transform.position);
            }

            // color-gradient separation lines
            lr.SetPosition(6, transform.position + transform.forward * 0.25f);
            lr.SetPosition(7, transform.position);

            if (targetInfo.cc > 0) // cohesion
            {
                
                lr.SetPosition(8, transform.position + (targetInfo.cd * Vector3.forward) * cohesionDist);
            }
            else {
                lr.SetPosition(8, transform.position);
            }
        }
    }

    public Gradient CalculateTargetLinesGradient(bool separationValid, bool alignmentValid, bool cohesionValid)
    {
        Gradient grad = new Gradient();
        float separatorLength = 1f;

        float sepVal = (separationValid ? separationDist : 0);
        float aliVal = (alignmentValid ? alignmentDist * 2 : 0); // remember to account for doubling done here
        float cohVal = (cohesionValid ? cohesionDist : 0);

        float totalLineDist = sepVal + aliVal + cohVal + separatorLength;

        GradientColorKey[] colKey = {
            new GradientColorKey(Color.red, ((sepVal + separatorLength*0.25f) / totalLineDist)),
            new GradientColorKey(Color.green, ((sepVal + aliVal + separatorLength*0.75f) / totalLineDist)),
            new GradientColorKey(Color.blue, 1.0f)
        };
        GradientAlphaKey[] alphKey = { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };

        grad.SetKeys(colKey, alphKey);
        grad.mode = GradientMode.Fixed;
        return grad;
    }

    public Gradient GenerateRedGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colKey = { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) };
        GradientAlphaKey[] alphKey = { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
        gradient.SetKeys(colKey, alphKey);
        
        return gradient;
    }

    public void ConfigureLineRenderer()
    {
        switch (debugMode)
        {
            case 0:
                lr.enabled = false;
                break;

            case 1:
                lr.positionCount = 2;
                lr.startWidth = 0.2f;
                lr.endWidth = 0.2f;
                lr.colorGradient = GenerateRedGradient();
                break;

            case 2:
                lr.positionCount = 10;
                lr.startWidth = 0.2f;
                lr.endWidth = 0.2f;
                lr.colorGradient = GenerateRedGradient();
                break;

            case 3:
                lr.positionCount = 9;
                lr.startWidth = 0.2f;
                lr.endWidth = 0.2f;
                lr.colorGradient = CalculateTargetLinesGradient(true, true, true);
                break;
        }
    }

    public bool InFOV(Transform val)
    {

        Vector3 relativePos = val.position - transform.position;
        float dist = relativePos.magnitude;
        if (dist < sightDist)
        {
            float dotProd = Vector3.Dot(transform.forward, relativePos.normalized);
            if (dotProd <= Mathf.Cos(fov * Mathf.PI / 360)) //checks against fov (converted into radians / 2)
                return true;
        }

        return false;
    }



    // Getters & Setters block
    public GameObject getTriContainer() { return triContainer; }
    public void setTriContainer(GameObject triContainer) { this.triContainer = triContainer; }

    public BoidScript[] getBoidArr() { return boidArr; }
    public void setBoidArr(BoidScript[] boidArr) { this.boidArr = boidArr; }

    public int getDebugMode() { return debugMode; }
    public void setDebugMode(int debugMode) { this.debugMode = debugMode; ConfigureLineRenderer(); }
}

// Used to store info about calculated target directions
public class TargetInfo
{
    public Quaternion sd, ad, cd;
    public int sc, ac, cc;

    public TargetInfo(Quaternion separationDir, Quaternion alignmentDir, Quaternion cohesionDir, int seperationCount, int alignmentCount, int cohesionCount)
    {
        sd = separationDir;
        ad = alignmentDir;
        cd = cohesionDir;
        sc = seperationCount;
        ac = alignmentCount;
        cc = cohesionCount;
    }

}