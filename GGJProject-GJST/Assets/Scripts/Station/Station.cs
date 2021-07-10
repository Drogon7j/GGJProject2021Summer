using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] private int targetAKeyDown = 5;
    [SerializeField] private int targetDKeyDown = 5;
    [SerializeField] private GameObject deceleratePrefab = null;
    [SerializeField] private GameObject deceleratePos = null;
    private int currentAKeyDown;
    private int currentDKeyDown;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (true)//other == player
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentAKeyDown >= targetAKeyDown)
                {
                    return;
                }
                currentAKeyDown++;
                Debug.Log("A:" + currentAKeyDown);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentDKeyDown >= targetDKeyDown)
                {
                    return;
                }
                currentDKeyDown++;
                Debug.Log("D:" + currentDKeyDown);
            }
        }
        if (currentAKeyDown == targetAKeyDown && currentDKeyDown == targetDKeyDown)
        {
            Debug.Log("Refresh");//生成减速光幕
            GameObject obj = Instantiate(deceleratePrefab,deceleratePos.transform);
            targetAKeyDown = 999;
            targetDKeyDown = 999;
        }
    }
}
