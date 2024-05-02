using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidScript : MonoBehaviour
{

    [SerializeField] private GameObject triContainer;
    [SerializeField] private Transform[] triArray;

    void Start()
    {
        for(int i = 0; i < triContainer.transform.childCount; i++){

        }
        triArray = triContainer.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        
    }
}
