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

    [SerializeField] private BoidScript selectedBoid;
    [SerializeField] private GameObject boidDebugModeObject;
    [SerializeField] private Camera[] camArr;
    private Camera activeCam;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        activeCam = camArr[0];
        foreach (BoidScript boid in this.gameObject.GetComponentsInChildren<BoidScript>())
        {
            boidList.Add(boid);
        }
        foreach (BoidScript boid in this.gameObject.GetComponentsInChildren<BoidScript>())
        {
            BoidScript[] filteredArr = boidList.Except(new[] { boid }).ToArray();
            boid.setBoidArr(filteredArr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectNewActiveBoid();
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
        Ray ray = activeCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.tag == "boid capsule")
            {
                selectedBoid.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
                hitObj.GetComponent<MeshRenderer>().material = selectedMaterial;
                selectedBoid = hitObj.GetComponentInParent<BoidScript>();
                camArr[1].gameObject.GetComponent<ActionCamera>().setTarget(selectedBoid); //directly references action camera in array
                boidDebugModeObject.GetComponent<TMP_InputField>().text = GetBoidDebugMode().ToString();
            }
        }
    }

    public void ToggleCamera()
    {
        activeCam.depth = 0;
        int currentIndex = Array.IndexOf(camArr, activeCam);
        activeCam = camArr[(currentIndex+1) % 2];
        activeCam.depth = 1;
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
        if (int.TryParse(val, out _))
        {
            selectedBoid.setDebugMode(int.Parse(val));
        }
    }
}
