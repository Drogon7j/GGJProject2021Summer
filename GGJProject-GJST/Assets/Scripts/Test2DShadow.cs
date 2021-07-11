using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test2DShadow : MonoBehaviour
{
    private void Awake()
    {
        transform.GetComponent<SpriteRenderer>().receiveShadows = true;
        transform.GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.TwoSided;
    }
}
