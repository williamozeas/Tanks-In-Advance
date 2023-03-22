using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RoundTimer : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update() {
        if(GameManager.Instance.GameState == GameStates.Playing){
            _tmp.text = Math.Floor(GameManager.Instance.RoundTime).ToString();
        }
    }

    private void OnEnable()
    {
        GameManager.OnRoundEnd += OnRoundEnd;
    }

    private void OnDisable()
    {
        GameManager.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundEnd()
    {
        _tmp.text = "";
    }
}
