using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BoidManager : MonoBehaviour
{

    [SerializeField] private TriManager triManagerObj;
    [SerializeField] private GameObject boidPrefab;

    [SerializeField] private GameObject boidHolder;
    [SerializeField] private List<BoidScript> boidList;

    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private BoidScript selectedBoid;
    [SerializeField] private GameObject boidDebugModeObject;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;

    // Cell Aut Variables
    private static int stepsPerCycle = 2;
    private static int envLo = 3, envHi = 4, fertLo = 4, fertHi = 5;
    private static bool countCorners = true;
    private AutManager autManager = new(envLo, envHi, fertLo, fertHi, countCorners);
    private BoidCellAutFunctions cellAutFuncs = new();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (BoidScript boid in this.gameObject.GetComponentsInChildren<BoidScript>())
        {
            boidList.Add(boid);
        }
        foreach (BoidScript boid in this.gameObject.GetComponentsInChildren<BoidScript>())
        {
            BoidScript[] filteredArr = boidList.Except(new[] { boid }).ToArray();
            boid.setBoidArr(filteredArr);
            boid.setSeed(UnityEngine.Random.Range(0, 1000));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectNewActiveBoid();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StepBoidsActiveTris();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PopulateBoidCellAut();
        }
    }

    public void SelectNewActiveBoid()
    {
        // exits function if click is on UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // raycast & variable declaration
        Ray ray = cameraManager.GetActiveCamera().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.tag == "boid capsule")
            {
                if (selectedBoid is not null)  selectedBoid.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
                hitObj.GetComponent<MeshRenderer>().material = selectedMaterial;
                selectedBoid = hitObj.GetComponentInParent<BoidScript>();
                cameraManager.GetActionCamera().gameObject.GetComponent<ActionCamera>().setTarget(selectedBoid); //directly references action camera in array
                boidDebugModeObject.GetComponent<TMP_InputField>().text = GetBoidDebugMode().ToString();

                triManagerObj.setActiveTris(selectedBoid.getActiveTris().ToList());
            }
        }
    }

    //specified function for boids
    public void StepBoidsActiveTris()
    {
        foreach(BoidScript boid in boidList)
        {
            List<TriController> activeTris = boid.getActiveTris().ToList();
            Debug.Log(activeTris.Count + " " + boid.name);
            for (int i = 0; i < stepsPerCycle; i++)
            {
                activeTris = autManager.Step(triManagerObj.getTriList(), activeTris);
                if(activeTris.Count > 0)
                    Debug.Log(activeTris[0] + " " + boid.name);
            }

            boid.setActiveTris(activeTris.ToArray());

            UpdateDisplayedTris(boid, activeTris);
        }
    }


    public void SetAutManagerRules(int envLo_new, int envHi_new, int fertLo_new, int fertHi_new, bool countCorners_new)
    {
        envLo = envLo_new;
        envHi = envHi_new;
        fertLo = fertLo_new;
        fertHi = fertHi_new;
        countCorners = countCorners_new;

        autManager.SetRules(envLo, envHi, fertLo, fertHi, countCorners);
    }

    public void UpdateSelectedBoidTris(List<TriController> activeTris)
    {
        if(selectedBoid is not null)
        {
            selectedBoid.setActiveTris(activeTris.ToArray());
        }
    }

    //general function using autManager for other scripts' access
    public List<TriController> StepCellAut(List<TriController> triList, List<TriController> activeTris)
    {
        return autManager.Step(triList, activeTris);
    }

    public void PopulateBoidCellAut()
    {
        List<TriController> triList = triManagerObj.getTriList();
        foreach(BoidScript boid in boidList)
        {
            List<TriController> activeTris = cellAutFuncs.PopulateCA(triList, boid.getRngBitArr(), boid.CalculateUrgencies());
            boid.setActiveTris(activeTris.ToArray());
            boid.setWeights(cellAutFuncs.CountActiveTrisInQuadrants(activeTris, triManagerObj.getTriList().Count()));
            UpdateDisplayedTris(boid, activeTris);
        }
    }

    public void UpdateDisplayedTris(BoidScript boid, List<TriController> activeTris)
    {
        if (boid == selectedBoid)
        {
            triManagerObj.setActiveTris(activeTris);
        }
    }

    public GameObject getBoidHolder() { return boidHolder; }
    public void setBoidHolder( GameObject boidHolder ) { this.boidHolder = boidHolder; }

    public List<BoidScript> GetList() { return boidList; }
    public void SetList(List<BoidScript> boidList) { this.boidList = boidList; }

    public void setBoidRefs(GameObject triContainerObj)
    {
        boidPrefab.GetComponent<BoidScript>().setTriContainer(triContainerObj);
    }

    public int GetBoidDebugMode() { return selectedBoid.getDebugMode(); }
    public void SetBoidDebugMode()
    {
        string val = boidDebugModeObject.GetComponent<TMP_InputField>().text;
        if (int.TryParse(val, out _) && selectedBoid is not null)
        {
            selectedBoid.setDebugMode(int.Parse(val));
        }
    }
}
