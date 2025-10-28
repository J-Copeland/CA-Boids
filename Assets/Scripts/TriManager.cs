using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriManager : MonoBehaviour
{

    //baseline GameObjects
    [SerializeField] private GameObject triUpPrefab;
    [SerializeField] private GameObject triDownPrefab;
    [SerializeField] private List<TriController> triList;
    [SerializeField] private GameObject visualPanelUiObj;
    [SerializeField] private TextMeshProUGUI visualPanelButtonText;
    [SerializeField] private BoidManager boidManager;

    //Unity specified data
    [SerializeField] private int rowNumber;
    private static int envLo = 3, envHi = 4, fertLo = 4, fertHi = 5;
    private static bool countCorners = true;
    [SerializeField] private GameObject envLoObject, envHiObject, fertLoObject, fertHiObject, cornersToggle, coordsToggle;
    //private AutManager autManager = new(envLo, envHi, fertLo, fertHi, countCorners); //logical upper integer limit of 12 if true, 3 if false
    [SerializeField] private bool displayCoords = true;
    [SerializeField] private string unusedButtonSymbol;

    //Stepping Variables
    [SerializeField] private TextMeshProUGUI stepperText;
    private Stack<List<TriController>> partialCheckpoint = new();
    private Stack<List<TriController>> partialRedoCheckpoint = new();
    [SerializeField] private List<TriController> activeTris;
    private int stepCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        triUpPrefab.GetComponent<TriController>().setTriManager(this);
        triDownPrefab.GetComponent<TriController>().setTriManager(this);
        Transform triHolder = visualPanelUiObj.transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        GenerateGrid(triHolder);

        foreach (TriController tri in triList)
        {
            CreateCornerAdjacencies(tri);
        }

        // establish default UI state for settings elements
        envLoObject.GetComponent<TMP_InputField>().text = envLo.ToString();
        envHiObject.GetComponent<TMP_InputField>().text = envHi.ToString();
        fertLoObject.GetComponent<TMP_InputField>().text = fertLo.ToString();
        fertHiObject.GetComponent<TMP_InputField>().text = fertHi.ToString();
        SetCountCorners(countCorners);
    }

    // Update is called once per frame
    void Update()
    {
        if (visualPanelUiObj.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.K)) StepForward();
            if (Input.GetKeyDown(KeyCode.J)) StepBackward();
        }
    }


    // Get/Set stack for UI settings buttons
    public int GetEnvLo() { return envLo; }
    public void SetEnvLo()
    {
        string val = envLoObject.GetComponent<TMP_InputField>().text;
        if (int.TryParse(val, out _))
        {
            envLo = int.Parse(val);
            boidManager.SetAutManagerRules(envLo, envHi, fertLo, fertHi, countCorners);
        }
    }

    public int GetEnvHi() { return envHi; }
    public void SetEnvHi()
    {
        string val = envHiObject.GetComponent<TMP_InputField>().text;
        if (int.TryParse(val, out _))
        {
            envHi = int.Parse(val);
            boidManager.SetAutManagerRules(envLo, envHi, fertLo, fertHi, countCorners);
        }
    }

    public int GetFertLo() { return fertLo; }
    public void SetFertLo()
    {
        string val = fertLoObject.GetComponent<TMP_InputField>().text;
        if (int.TryParse(val, out _))
        {
            fertLo = int.Parse(val);
            boidManager.SetAutManagerRules(envLo, envHi, fertLo, fertHi, countCorners);
        }
    }

    public int GetFertHi() { return fertHi; }
    public void SetFertHi()
    {
        string val = fertHiObject.GetComponent<TMP_InputField>().text;
        if (int.TryParse(val, out _))
        {
            fertHi = int.Parse(val);
            boidManager.SetAutManagerRules(envLo, envHi, fertLo, fertHi, countCorners);
        }
    }

    public bool GetCountCorners() { return countCorners; }
    public void SetCountCorners(bool state)
    {
        countCorners = state;
        if (countCorners)
        {
            cornersToggle.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            cornersToggle.GetComponent<Image>().color = Color.white;
        }
        boidManager.SetAutManagerRules(envLo, envHi, fertLo, fertHi, countCorners);
    }
    public void ToggleCountCorners()
    {
        SetCountCorners(!countCorners);
    }

    public bool GetDisplayCoords() { return displayCoords; }
    public void SetDisplayCoords(bool state)
    {
        displayCoords = state;
        if (displayCoords)
        {
            coordsToggle.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            coordsToggle.GetComponent<Image>().color = Color.white;
        }

        foreach (TriController tri in triList)
        {
            tri.setTextState(displayCoords);
        }

    }
    public void ToggleDisplayCoords()
    {
        SetDisplayCoords(!displayCoords);
    }

    public void ResetActiveTriList()
    {
        while (activeTris.Count > 0) activeTris[0].setStateFalse();
    }

    public void ResetStepCounter()
    {
        stepCount = 0;
        stepperText.text = "Steps: " + stepCount;

        partialCheckpoint = new Stack<List<TriController>>();
    }

    public void ToggleVisualPanel()
    {
        visualPanelUiObj.SetActive(!visualPanelUiObj.activeSelf);

        string tempHolder = visualPanelButtonText.text;
        visualPanelButtonText.text = unusedButtonSymbol;
        unusedButtonSymbol = tempHolder;
    }



    // General Get/Set stack
    public List<TriController> GetList() { return triList; }

    public void SetList(TriController[] newArr) { triList = newArr.ToList(); }


    private void GenerateGrid(Transform holder)
    {
        int quadrantSize = rowNumber / 2;

        for (int i = 0; i < rowNumber; i++)
        {
            GameObject currentTriDown = null;
            TriController downController = null;

            for (int j = 0; j <= i; j++)
            {
                GameObject currentTriUp = Instantiate(triUpPrefab, holder);

                float scaleFactor = (currentTriUp.GetComponent<RectTransform>().localScale.x / rowNumber);
                currentTriUp.GetComponent<RectTransform>().localScale /= rowNumber;
                currentTriUp.GetComponent<RectTransform>().localPosition = CalculateTriPos(i, j, 0, rowNumber, scaleFactor);

                TriController upController = currentTriUp.GetComponent<TriController>();
                upController.Populate(displayCoords, i - j, j, CalculateQuadrant(i, j, 0, quadrantSize).ToString());
                triList.Add(upController);

                //set adj from right up-pointy to left if exists
                if (downController != null)
                {
                    CreateAdjacencies(upController, downController);
                }


                if (j != i)
                {
                    //establish down-pointy
                    currentTriDown = Instantiate(triDownPrefab, holder);

                    currentTriDown.GetComponent<RectTransform>().localScale /= rowNumber;
                    currentTriDown.GetComponent<RectTransform>().localPosition = CalculateTriPos(i, j, 1, rowNumber, scaleFactor);

                    downController = currentTriDown.GetComponent<TriController>();
                    downController.Populate(displayCoords, i - j - 1, j, CalculateQuadrant(i, j, 1, quadrantSize).ToString());
                    triList.Add(downController);

                    //set adj from left up-pointy
                    CreateAdjacencies(upController, downController);
                    //set adj from down-pointy to tri upwards
                    CreateAdjacencies(downController, triList[triList.Count - i * 2 - 1]);

                }
            }
        }


    }

    enum Quadrant
    {
        top,
        left,
        right,
        center
    }

    private Quadrant CalculateQuadrant(int i, int j, int polarityOffset, int quadrantSize)
    {
        if(i < quadrantSize)
            return Quadrant.top;
        if (i-j-polarityOffset >= quadrantSize + quadrantSize % 2) // + 0/1 to account for odd number of rows
            return Quadrant.left;
        if (j >= quadrantSize + quadrantSize % 2) // + 0/1 to account for odd number of rows
            return Quadrant.right;
        return Quadrant.center;
    }

    private Vector3 CalculateTriPos(int i, int j, int polarityOffset, int rowNumber, float scaleFactor)
    {
        //-86.6 ~= (100sqrt3)/2 (triangle height)
        return new Vector3((100 * j) - (100 * i / 2) + 50*polarityOffset, (-86.6f * i) + ((rowNumber - 1) * 43.3f), 0) * scaleFactor;
    }


    //creates adjacencies both ways - couldn't think of a reason to only have one way
    private void CreateAdjacencies(TriController tri1, TriController tri2)
    {
        List<TriController> currentAdj = new(tri1.getAdjTris());
        currentAdj.Add(tri2);
        tri1.setAdjTris(currentAdj.ToArray());

        currentAdj = new List<TriController>(tri2.getAdjTris());
        currentAdj.Add(tri1);
        tri2.setAdjTris(currentAdj.ToArray());
    }


    private void CreateCornerAdjacencies(TriController tri)
    {
        //part 1
        List<TriController> cornerAdjacencies = new();
        TriController[] sideAdjacencies = tri.getAdjTris();
        for(int i = 0; i < sideAdjacencies.Length; i++)
        {
            TriController[] tempAdj = sideAdjacencies[i].getAdjTris();
            for (int j = 0; j < tempAdj.Length; j++) //time inefficient
            {
                if(tempAdj[j] != tri)   cornerAdjacencies.Add(tempAdj[j]);
            }
        }
        cornerAdjacencies = cornerAdjacencies.Distinct().ToList();
        
        //new list for part 2
        List<TriController> secondaryCornerAdjacencies = new();
        Dictionary<TriController, int> cornerAdjCounter = new();

        foreach(TriController adjTri in cornerAdjacencies)
        {
            TriController[] adjTriArr = adjTri.getAdjTris();
            foreach(TriController potentialAdj in adjTriArr)
            {
                if (cornerAdjCounter.ContainsKey(potentialAdj))
                    cornerAdjCounter[potentialAdj] += 1;
                else
                    cornerAdjCounter.Add(potentialAdj, 1);
            }
        }

        for(int i = 0; i < cornerAdjCounter.Count; i++)
        {
            if(cornerAdjCounter.ElementAt(i).Value > 1)
            {
                cornerAdjacencies.Add(cornerAdjCounter.ElementAt(i).Key);
            }
        }

        cornerAdjacencies = cornerAdjacencies.Except(sideAdjacencies).ToList();
        tri.setCornerAdjTris(cornerAdjacencies.ToArray());
    }

    public List<TriController> getTriList()
    {
        return triList;
    }

    public void AddTri(TriController newTri)
    {
        if (!activeTris.Contains(newTri))
        {
            activeTris.Add(newTri);
            boidManager.UpdateSelectedBoidTris(activeTris);
        }
    }


    public void RemoveTri(TriController newTri)
    {
        activeTris.Remove(newTri);
        boidManager.UpdateSelectedBoidTris(activeTris);
    }

    public List<TriController> getActiveTris()
    {
        return activeTris;
    }
    public void setActiveTris(List<TriController> activeTris)
    {
        ResetActiveTriList();
        foreach(TriController tri in activeTris)
        {
            tri.setStateTrue();
        }
        boidManager.UpdateSelectedBoidTris(activeTris);
    }


    //save-state stepping only works 1 way.
    //more complex design needed for 2-way, future and past tris obfuscating other tris leads to incorrect deletions etc.
    private void StepForward()
    {
        stepCount++;
        stepperText.text = "Steps: " + stepCount;

        List<TriController> trisToAdd = boidManager.StepCellAut(triList, activeTris);
        TriController[] trisToRemove = activeTris.ToArray();

        //push changes to undo stack
        partialCheckpoint.Push(activeTris.ToList());

        //push changes (in order of processes)
        foreach (TriController tri in trisToRemove) tri.setStateFalse();
        foreach (TriController tri in trisToAdd) tri.setStateTrue();
    }


    private void StepBackward()
    {
        if (stepCount > 0)
        {

            stepCount--;
            stepperText.text = "Steps: " + stepCount;

            //clear grid
            while (activeTris.Count > 0) activeTris[0].setStateFalse();

            //load tris
            List<TriController> trisToAdd = partialCheckpoint.Pop();
            foreach (TriController tri in trisToAdd) tri.setStateTrue();


            //---------------------2-WAY COMPATABLE VERSION------------------------

            ////activate tris
            //List<TriController> trisToAdd = partialCheckpoint.Pop();


            ////deactivate tris
            //List <TriController> trisToRemove = new List<TriController>();
            //for (int i = 0; i < activeTris.Count; i++)
            //{
            //    int activeAdjs = 0;
            //    for (int j = 0; j < activeTris[i].getAdjTris().Length; j++)
            //    {
            //        if (activeTris[i].getAdjTris()[j].getState()) activeAdjs++;
            //    }
            //    //Debug.Log("(" + activeTris[i].getI() + "," + activeTris[i].getJ() + "," + activeTris[i].getPolarity() + ") has: " + activeAdjs);
            //    if (activeAdjs < 2)
            //    {
            //        trisToRemove.Add(activeTris[i]);
            //    }
            //}

            //if(partialCheckpoint.Count > 0)
            //{
            //    trisToRemove.Except(partialCheckpoint.Peek());
            //}


            //push changes to redo stack
            //Debug.Log("pushing: " + trisToRemove + " to redo. Now on: " + partialRedoCheckpoint.Count + " layers.");

            ////push changes (in order of processes)
            //foreach (TriController tri in trisToAdd) tri.setStateTrue();
            //foreach (TriController tri in trisToRemove) tri.setStateFalse();
        }
    }
}
