using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutManager
{

    private int El; //environmental low
    private int Eh; //environmental high
    private int Fl; //fertility low
    private int Fh; //fertility high
    private bool cornerAdjacencies;

    public AutManager(int El, int Eh, int Fl, int Fh, bool cornerAdjacencies)
    {
        this.El = El;
        this.Eh = Eh;
        this.Fl = Fl;
        this.Fh = Fh;
        this.cornerAdjacencies = cornerAdjacencies;
    }

    public List<TriController> Step(List<TriController> allTris, List<TriController> activeTris)
    {
        //Hotswappable function
        return ModularCellAut(allTris, activeTris);
    }

    private List<TriController> CornerAdjacentGrowth(List<TriController> allTris, List<TriController> activeTris)
    {
        List<TriController> toAdd = new List<TriController>();
        foreach (TriController tri in activeTris)
        {
            toAdd.AddRange(tri.getCornerAdjTris());
        }
        return toAdd.Distinct().ToList();
    }

    private List<TriController> OriginalAlgorithm(List<TriController> allTris, List<TriController> activeTris)
    {
        List<TriController> toAdd = new List<TriController>();

        for (int i = 0; i < allTris.Count; i++)
        {
            int activeAdjs = 0;
            for (int j = 0; j < allTris[i].getAdjTris().Length; j++)
            {
                if (allTris[i].getAdjTris()[j].getState()) activeAdjs++;
            }

            if (activeAdjs >= 2)
            {
                toAdd.Add(allTris[i]);
            }
        }

        return toAdd;
    }

    private List<TriController> ModularCellAut(List<TriController> allTris, List<TriController> activeTris)
    {
        List<TriController> toAdd = new List<TriController>();

        if(cornerAdjacencies == false)
        {
            for (int i = 0; i < allTris.Count; i++)
            {
                int activeAdjs = 0;
                for (int j = 0; j < allTris[i].getAdjTris().Length; j++)
                {
                    if (allTris[i].getAdjTris()[j].getState()) activeAdjs++;
                }

                if (allTris[i].getState() == false && activeAdjs >= Fl && activeAdjs <= Fh) //fertility check
                {
                    toAdd.Add(allTris[i]);
                }

                if (allTris[i].getState() == true && activeAdjs >= El && activeAdjs <= Eh) //environmental survival check
                {
                    toAdd.Add(allTris[i]);
                }
            }
        }
        else //repeated code, but only does cornerAdj. check once
        {
            for (int i = 0; i < allTris.Count; i++)
            {
                int activeAdjs = 0;
                for (int j = 0; j < allTris[i].getAdjTris().Length; j++)
                {
                    if (allTris[i].getAdjTris()[j].getState()) activeAdjs++;
                }

                for (int j = 0; j < allTris[i].getCornerAdjTris().Length; j++)
                {
                    if (allTris[i].getCornerAdjTris()[j].getState()) activeAdjs++;
                }

                if (allTris[i].getState() == false && activeAdjs >= Fl && activeAdjs <= Fh) //fertility check
                {
                    toAdd.Add(allTris[i]);
                }

                if (allTris[i].getState() == true && activeAdjs >= El && activeAdjs <= Eh) //environmental survival check
                {
                    toAdd.Add(allTris[i]);
                }
            }
        }
        

        return toAdd;
    }
    

    /*-USED PAPER:----------------------------------------------------------------
    ---"Cellular Automata in the Triangular Tesselation" by Carter Bays (1994)----
    ---FOR REFERENCE------------------------------------------------------------*/
}
