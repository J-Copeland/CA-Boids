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
    [SerializeField] private AutManager autManager = new AutManager(2,3,5,6,true); //logical upper integer limit of 12 if true, 3 if false

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
        if (Input.GetKeyDown("k")) StepForward();
        if (Input.GetKeyDown("j")) StepBackward();
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
        
        //reset list for round 2
        List<TriController> secondaryCornerAdjacencies = new List<TriController>();
        for (int w = 0; w < cornerAdjacencies.Count; w++)
        {
            for (int x = 0; x < cornerAdjacencies[w].getAdjTris().Length; x++)
            {
                for (int y = 0; y < cornerAdjacencies.Count; y++)
                {
                    for (int z = 0; z < cornerAdjacencies[y].getAdjTris().Length; z++) //fix error
                    {
                        if (cornerAdjacencies[w] != cornerAdjacencies[y] && cornerAdjacencies[w].getAdjTris()[x] == cornerAdjacencies[y].getAdjTris()[z]) //please rewrite this soon
                        {
                            secondaryCornerAdjacencies.Add(cornerAdjacencies[w].getAdjTris()[x]);
                        }
                    }
                }
            }
        }

        secondaryCornerAdjacencies = secondaryCornerAdjacencies.Distinct().ToList();
        secondaryCornerAdjacencies = secondaryCornerAdjacencies.Except(sideAdjacencies).ToList();
        cornerAdjacencies.AddRange(secondaryCornerAdjacencies);
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

    //stepping only works 1 way.
    //more complex design needed for 2-way, future and past tris obfuscating other tris leads to incorrect deletions etc.
    private void StepForward()
    {
        stepCount++;
        stepperText.text = "Steps: " + stepCount;

        ////SECTION: ACTIVATE TRIS
        //List<TriController> trisToAdd = new List<TriController>();

        ////logic loop for cellaut
        //for (int i = 0; i < triList.Count; i++)
        //{
        //    int activeAdjs = 0;
        //    for (int j = 0; j < triList[i].getAdjTris().Length; j++)
        //    {
        //        if (triList[i].getAdjTris()[j].getState()) activeAdjs++;
        //    }

        //    if (activeAdjs >= 2)
        //    {
        //        trisToAdd.Add(triList[i]);
        //    }
        //}

        List<TriController> trisToAdd = autManager.Step(triList, activeTris);

        //SECTION: DEACTIVATE TRIS
        List<TriController> trisToRemove = new List<TriController>();

        //logic loop for reverse cellaut
        for (int i = 0; i < activeTris.Count; i++)
        {
            int activeAdjs = 0;
            for (int j = 0; j < activeTris[i].getAdjTris().Length; j++)
            {
                if (activeTris[i].getAdjTris()[j].getState()) activeAdjs++;
            }
            //Debug.Log("(" + activeTris[i].getJ() + "," + activeTris[i].getK() + "," + activeTris[i].getPolarity() + ") has: " + activeAdjs);
            if (activeAdjs < 2)
            {
                trisToRemove.Add(activeTris[i]);
            }
        }

        
        
        //push changes to undo stack
        partialCheckpoint.Push(activeTris.ToList());

        Debug.Log("pushing: " + activeTris.Count + " to undo. Now on: " + partialCheckpoint.Count + " layers.");

        //push changes (in order of processes)
        foreach (TriController tri in trisToAdd) tri.setStateTrue();
        foreach (TriController tri in trisToRemove) tri.setStateFalse();
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
