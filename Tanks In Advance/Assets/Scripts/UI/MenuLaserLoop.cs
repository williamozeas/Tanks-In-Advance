using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = System.Random;

public class MenuLaserLoop : MonoBehaviour
{
    public float timeElapsed = 0;
    private VisualEffect VFX;

    private void Start()
    {
        VFX = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > 7)
        VFX.Play();
    }
}
