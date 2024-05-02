using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriController : MonoBehaviour
{
    [SerializeField] private TriManager triManager;
    [SerializeField] private int jCoord;
    [SerializeField] private int kCoord;
    [SerializeField] private bool polarity;
    [SerializeField] private bool state = false;
    [SerializeField] private TriController[] adjTris;
    private Image image;
    private float alphaThreshold = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.white;
        image.alphaHitTestMinimumThreshold = alphaThreshold;

        List<TriController> allTris = triManager.GetList();
        //adjTris = CalculateAdjacencies(allTris);



    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getJ() { return jCoord; }
    public void setJ(int newJ) { jCoord = newJ; }

    public int getK() { return kCoord; }
    public void setK(int newK) { kCoord = newK; }

    public bool getPolarity() { return polarity; }
    public void setPolarity(bool newVal) { polarity = newVal; }

    public bool getState() { return state; }
    public void setState(bool newState) { state = newState; StateCheck(); }
    public void setStateTrue() { state = true; StateCheck(); }
    public void setStateFalse() { state = false; StateCheck(); }
    public void StateFlip() { state = !state; StateCheck(); }

    public TriManager getTriManager() { return triManager; }
    public void setTriManager(TriManager triManager) { this.triManager = triManager; }

    public TriController[] getAdjTris() { return adjTris; }
    public void setAdjTris(TriController[] adjTris) { this.adjTris = adjTris; }

    

    private void StateCheck()
    {
        if (state)
        {
            image.color = Color.red;
            triManager.AddTri(this);
        }
        else
        {
            image.color = Color.white;
            triManager.RemoveTri(this);
        }
    }

}
