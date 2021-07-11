using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    public Camera mainCamera;

    private List<Transform> backgroundSprites;

    private void Awake()
    {
        backgroundSprites = new List<Transform>();
        for (var i = 0; i < transform.childCount; i++)
        {
            backgroundSprites.Add(transform.GetChild(i));
        }
    }

    private void Update()
    {
        if (mainCamera != null)
        {
            float cPosX = mainCamera.transform.position.x;
            float cPosY = mainCamera.transform.position.y;
            if (cPosX >= -20f && cPosX <= 20f && cPosY >= -20f &&
                cPosY <= 20f)
            {
                // Origin
                for (var i = 0; i < 9; i++)
                {
                    SetupRelativePos(i, new Vector3(0f, 0f, 0.2f));
                }
            }
            else
            {
                Vector3 origin = new Vector3(0f, 0f, 0.2f);
                if (cPosX < -20f)
                {
                    origin.x = Mathf.Ceil((cPosX + 20f) / -40f) * -40f;
                }

                if (cPosX > 20f)
                {
                    origin.x = Mathf.Ceil((cPosX - 20f) / 40f) * 40f;
                }

                if (cPosY < -20f)
                {
                    origin.y = Mathf.Ceil((cPosY + 20f) / -40f) * -40f;
                }

                if (cPosY > 20f)
                {
                    origin.y = Mathf.Ceil((cPosY - 20f) / 40f) * 40f;
                }

                for (var i = 0; i < 9; i++)
                {
                    SetupRelativePos(i, origin);
                }
            }
        }
    }

    private void SetupRelativePos(int dir, Vector3 origin)
    {
        Vector3 result = Vector3.zero;
        switch (dir)
        {
            case 0: // LB
                result = origin;
                result.x += -40f;
                result.y += -40f;
                break;
            case 1: // B
                result = origin;
                result.x += 0f;
                result.y += -40f;
                break;
            case 2: // RB
                result = origin;
                result.x += 40f;
                result.y += -40f;
                break;
            case 3: // L
                result = origin;
                result.x += -40f;
                result.y += 0f;
                break;
            case 4: // C
                result = origin;
                result.x += 0f;
                result.y -= 0f;
                break;
            case 5: // R
                result = origin;
                result.x += 40f;
                result.y += 0f;
                break;
            case 6: // LT
                result = origin;
                result.x += -40f;
                result.y += 40f;
                break;
            case 7:
                result = origin;
                result.x += 0f;
                result.y += 40f;
                break;
            case 8: // RT
                result = origin;
                result.x += 40f;
                result.y += 40f;
                break;
        }
        result.z = 0.2f;
        backgroundSprites[dir].position = result;
    }
}

