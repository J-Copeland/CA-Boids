using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriController : MonoBehaviour
{
    [SerializeField] private TriManager triManager;
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] private int iCoord;
    [SerializeField] private int jCoord;
    [SerializeField] private bool polarity;
    [SerializeField] private string quadrant;
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
        text = this.GetComponentInChildren<TextMeshProUGUI>();
        
    }
    public void Populate(bool textState, int iCoord, int jCoord, string quadrant)
    {
        text.gameObject.SetActive(textState);
        this.iCoord = iCoord;
        this.jCoord = jCoord;
        text.text = iCoord + ", " + jCoord;
        this.quadrant = quadrant;
    }


    public int getI() { return iCoord; }
    public void setI(int newJ) { iCoord = newJ; text.text = iCoord + ", " + jCoord; }

    public int getJ() { return jCoord; }
    public void setJ(int newJ) { jCoord = newJ; text.text = iCoord + ", " + jCoord; }

    public bool getPolarity() { return polarity; }
    public void setPolarity(bool newVal) { polarity = newVal; }

    public string getQuadrant() { return quadrant; }
    public void setQuadrant(string quadrant) { this.quadrant = quadrant; }

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

    public bool getTextState() { return text.IsActive();  }
    public void setTextState(bool state) { text.gameObject.SetActive(state);  }



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
