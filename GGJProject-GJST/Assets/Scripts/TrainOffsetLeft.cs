using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOffsetLeft : MonoBehaviour
{
    // Start is called before the first frame update
    public int Offset = 1;
    private void Update()
    {

    }
    
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<TrainController>().GoOffset(Offset);
        Debug.Log(1);
    }
}
