using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.OnGameEnd += OnGameEnd;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameEnd -= OnGameEnd;
    }

    private void OnGameEnd(PlayerNum winner)
    {
        switch (winner)
        {
            case (PlayerNum.Player1):
            {
                tmp.text = "Blue Wins!";
                break;
            }
            case (PlayerNum.Player2):
            {
                tmp.text = "Pink Wins!";
                break;
            }
            case (PlayerNum.Neither):
            {
                tmp.text = "Tie!";
                break;
            }
        }
        
    }
}
