using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerText : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnGameEnd += OnGameEnd;
    }

    private void OnGameEnd()
    {
        
    }
}
