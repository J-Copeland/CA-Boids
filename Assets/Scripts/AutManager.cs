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
        SetRules(El, Eh, Fl, Fh, cornerAdjacencies);
    }

    public void SetRules(int El, int Eh, int Fl, int Fh, bool cornerAdjacencies)
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
        List<TriController> toAdd = new();
        foreach (TriController tri in activeTris)
        {
            toAdd.AddRange(tri.getCornerAdjTris());
        }
        return toAdd.Distinct().ToList();
    }

    private List<TriController> ModularCellAut(List<TriController> allTris, List<TriController> activeTris) //update to use activeTris instead of checking individual tri state -> should only be used for displaying on grid
    {
        List<TriController> toAdd = new();

        if(cornerAdjacencies == false)
        {

            foreach(TriController currentTri in allTris)
            {
                int activeAdjs = 0;
                for (int j = 0; j < currentTri.getAdjTris().Length; j++)
                {
                    if (activeTris.Contains(currentTri.getAdjTris()[j])) activeAdjs++;
                }

                if (currentTri.getState() == false && activeAdjs >= Fl && activeAdjs <= Fh) //fertility check
                {
                    toAdd.Add(currentTri);
                }

                if (currentTri.getState() == true && activeAdjs >= El && activeAdjs <= Eh) //environmental survival check
                {
                    toAdd.Add(currentTri);
                }
            }
        }
        else //repeated code, but only does cornerAdj. check once
        {
            foreach (TriController currentTri in allTris)
            {
                int activeAdjs = 0;
                for (int j = 0; j < currentTri.getAdjTris().Length; j++)
                {
                    if (activeTris.Contains(currentTri.getAdjTris()[j])) activeAdjs++;
                }

                for (int j = 0; j < currentTri.getCornerAdjTris().Length; j++)
                {
                    if (activeTris.Contains(currentTri.getCornerAdjTris()[j])) activeAdjs++;
                }


                bool currentState = activeTris.Contains(currentTri);


                if (currentState == false && activeAdjs >= Fl && activeAdjs <= Fh) //fertility check
                {
                    toAdd.Add(currentTri);
                }

                else if (currentState) //environmental survival check
                {
                    if (activeAdjs >= El && activeAdjs <= Eh)
                    {
                        toAdd.Add(currentTri);
                    }
                    //guard seperation allows for alive->dead tri processing here
                }
            }
                
        }
        

        return toAdd;
    }


    /*-USED PAPER:---------------------------------------------------------------------------------------
    ---"Cellular Automata in the Triangular Tesselation" by Carter Bays (1994)---------------------------
    ---"Triangular Automata: The 256 Elementary Cellular Automata of the 2D Plane" by Paul Cousin(2023)--
    ---FOR REFERENCE-----------------------------------------------------------------------------------*/
}
