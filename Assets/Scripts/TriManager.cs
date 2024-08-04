using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TriManager : MonoBehaviour
{

    //baseline GameObjects
    [SerializeField] private GameObject triUpPrefab;
    [SerializeField] private GameObject triDownPrefab;
    [SerializeField] private List<TriController> triList;
    [SerializeField] private GameObject canvas;

    //Unity specified data
    [SerializeField] private int rowNumber;
    [SerializeField] private AutManager autManager = new AutManager(2,3,3,3,true); //logical upper integer limit of 12 if true, 3 if false

    //Stepping Variables
    [SerializeField] private TextMeshProUGUI stepperText;
    [SerializeField] private Stack<List<TriController>> partialCheckpoint = new Stack<List<TriController>>();
    [SerializeField] private Stack<List<TriController>> partialRedoCheckpoint = new Stack<List<TriController>>();
    [SerializeField] private List<TriController> activeTris;
    private int stepCount;

    // Start is called before the first frame update
    void Start()
    {
        triUpPrefab.GetComponent<TriController>().setTriManager(this);
        triDownPrefab.GetComponent<TriController>().setTriManager(this);
        Transform triHolder = canvas.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Transform>();
        GenerateGrid(triHolder);

        foreach (TriController tri in triList)
        {
            CreateCornerAdjacencies(tri);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) StepForward();
        if (Input.GetKeyDown(KeyCode.J)) StepBackward();
    }

    public List<TriController> GetList() { return triList; }

    public void SetList(List<TriController> newList) { triList = newList; }


    private void GenerateGrid(Transform holder)
    {
        for(int i = 0; i < rowNumber; i++)
        {
            GameObject currentTriDown = null;
            TriController downController = null;
            for (int j = 0; j <= i; j++)
            {
                GameObject currentTriUp = Instantiate(triUpPrefab, holder);
                float scaleFactor = (currentTriUp.GetComponent<RectTransform>().localScale.x / rowNumber);
                currentTriUp.GetComponent<RectTransform>().localScale = currentTriUp.GetComponent<RectTransform>().localScale / rowNumber;
                currentTriUp.GetComponent<RectTransform>().localPosition = new Vector3((100 * j) - (100 * i / 2), (-86.6f * i) + ((rowNumber - 1) * 43.3f), 0) * scaleFactor; //-86.6 ~= (100sqrt3)/2 (triangle height)
                TriController upController = currentTriUp.GetComponent<TriController>();
                upController.setJ(i-j);
                upController.setK(j);
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
                    currentTriDown.GetComponent<RectTransform>().localScale = currentTriDown.GetComponent<RectTransform>().localScale / rowNumber;
                    currentTriDown.GetComponent<RectTransform>().localPosition = new Vector3((100 * j) - (100 * i / 2) + 50, (-86.6f * i) + ((rowNumber-1) * 43.3f), 0) * scaleFactor;
                    downController = currentTriDown.GetComponent<TriController>();
                    downController.setJ(i-j-1);
                    downController.setK(j);
                    triList.Add(downController);

                    //set adj from left up-pointy
                    CreateAdjacencies(upController, downController);
                    //set adj from down-pointy to tri upwards
                    CreateAdjacencies(downController, triList[triList.Count - i * 2 - 1]);

                }
            }
        }

        
    }


    //creates adjacencies both ways - couldn't think of a reason to only have one way
    private void CreateAdjacencies(TriController tri1, TriController tri2)
    {
        List<TriController> currentAdj = new List<TriController>(tri1.getAdjTris());
        currentAdj.Add(tri2);
        tri1.setAdjTris(currentAdj.ToArray());

        currentAdj = new List<TriController>(tri2.getAdjTris());
        currentAdj.Add(tri1);
        tri2.setAdjTris(currentAdj.ToArray());
    }


    private void CreateCornerAdjacencies(TriController tri)
    {
        //part 1
        List<TriController> cornerAdjacencies = new List<TriController>();
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
        List<TriController> secondaryCornerAdjacencies = new List<TriController>();
        Dictionary<TriController, int> cornerAdjCounter = new Dictionary<TriController, int>();

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

    public void AddTri(TriController newTri)
    {
        if(!activeTris.Contains(newTri)) activeTris.Add(newTri);
    }


    public void RemoveTri(TriController newTri)
    {
        activeTris.Remove(newTri);
    }


    //save-state stepping only works 1 way.
    //more complex design needed for 2-way, future and past tris obfuscating other tris leads to incorrect deletions etc.
    private void StepForward()
    {
        stepCount++;
        stepperText.text = "Steps: " + stepCount;

        List<TriController> trisToAdd = autManager.Step(triList, activeTris);
        TriController[] trisToRemove = activeTris.ToArray();

        //push changes to undo stack
        partialCheckpoint.Push(activeTris.ToList());

        //push changes (in order of processes)
        foreach (TriController tri in trisToRemove) tri.setStateFalse();
        foreach (TriController tri in trisToAdd) tri.setStateTrue();
    }


    private void StepBackward()
    {
        if(stepCount > 0)
        {

            stepCount--;
            stepperText.text = "Steps: " + stepCount;

            //clear grid
            while(activeTris.Count > 0) activeTris[0].setStateFalse();

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
            //    //Debug.Log("(" + activeTris[i].getJ() + "," + activeTris[i].getK() + "," + activeTris[i].getPolarity() + ") has: " + activeAdjs);
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
