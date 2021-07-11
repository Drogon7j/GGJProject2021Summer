using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TrainController m_TrainController = null;
    [SerializeField] private GameObject UIObj = null;
    public bool GameOver = false;

    private void Update()
    {
        Debug.Log(m_TrainController.OffsetDistance);
        if (m_TrainController.OffsetDistance >= 5)
        {
            GameOver = true;
        }

        if (GameOver)
        {
            GameOver = false;
            UIObj.SetActive(true);
        }
    }
}
