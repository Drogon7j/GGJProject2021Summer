using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField]private GameManager m_GameManager = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        m_GameManager.GameOver = true;
    }
}
