using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriController : MonoBehaviour
{
    [SerializeField] private TriManager triManager;
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] private int jCoord;
    [SerializeField] private int kCoord;
    [SerializeField] private bool polarity;
    [SerializeField] private bool state = false;
    [SerializeField] private TriController[] adjTris;
    [SerializeField] private TriController[] cornerAdjTris;
    private Image image;
    private float alphaThreshold = 0.1f;


    void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.white;
        image.alphaHitTestMinimumThreshold = alphaThreshold;
        text = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        
    }


    public int getJ() { return jCoord; }
    public void setJ(int newJ) { jCoord = newJ; text.text = jCoord + ", " + kCoord; }

    public int getK() { return kCoord; }
    public void setK(int newK) { kCoord = newK; text.text = jCoord + ", " + kCoord; }

    public bool getPolarity() { return polarity; }
    public void setPolarity(bool newVal) { polarity = newVal; }

    public bool getState() { return state; }
    public void setState(bool newState) { state = newState; StateCheck(); }
    public void setStateTrue() { state = true; StateCheck();  }
    public void setStateFalse() { state = false; StateCheck(); }
    public void StateFlip() { state = !state; StateCheck(); }

    public TriManager getTriManager() { return triManager; }
    public void setTriManager(TriManager triManager) { this.triManager = triManager; }

    public TriController[] getAdjTris() { return adjTris; }
    public void setAdjTris(TriController[] adjTris) { this.adjTris = adjTris; }

    public TriController[] getCornerAdjTris() { return cornerAdjTris; }
    public void setCornerAdjTris(TriController[] cornerAdjTris) { this.cornerAdjTris = cornerAdjTris; }



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
