using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOffsetRight : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<TrainController>().GoOffset(-1);
    }
}
