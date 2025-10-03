using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCellAutFunctions
{

    int checksPerTri = 10;
    int triCount = 25;
    int rngStrCount = 100;

    public List<TriController> PopulateCA(BoidScript boid, List<TriController> triList)
    {
        List<TriController> activeTris = new();

        byte[] bitArr = boid.getRngBitArr();
        for (int i = 0; i < triList.Count; i++)
        {

            int triTotal = 0;
            for(int j = 0; j < checksPerTri; j++)
            {
                Debug.Log((i * checksPerTri + j) % rngStrCount);
                if (bitArr[(i * checksPerTri + j) % rngStrCount] == 1)
                {
                    triTotal++;
                }
            }
            if(triTotal >= 5)
            {
                activeTris.Add(triList[i]);
            }
        }
        boid.setActiveTris(activeTris.ToArray());

        return activeTris;
    }
}
