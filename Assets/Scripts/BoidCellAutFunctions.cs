using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCellAutFunctions
{

    int checksPerTri = 10;
    int rngStrCount = 100;

    public List<TriController> PopulateCA(List<TriController> triList, byte[] bitArr, float[] urgencyArr)
    {
        List<TriController> activeTris = new();

        for (int i = 0; i < triList.Count; i++)
        {

            int triTotal = 0;
            for(int j = 0; j < checksPerTri; j++)
            {
                if (bitArr[(i * checksPerTri + j) % rngStrCount] == 1)
                {
                    triTotal++;
                }
            }

            int n = triList[i].getQuadrant() switch //pattern match to get urgency array index from quadrant
            {
                "top" => 0,
                "left" => 1,
                "right" => 2,
                _ => 3
            };

            float boundaryVal = checksPerTri * urgencyArr[n];

            if(triTotal >= boundaryVal)
            {
                activeTris.Add(triList[i]);
            }
        }

        return activeTris;
    }

    public float[] CountActiveTrisInQuadrants(List<TriController> activeTris, int triCount)
    {


        return new float[] {0f, 0f, 1f, 1f}; // currently split into {separation, alignment, cohesion, unused}
    }
}
