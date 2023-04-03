using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundTimerBar : MonoBehaviour
{
    private float maxX = 331.4134f;
    private float maxWidth = 21349.58f;

    private float minX = -514.0803f;
    private float minWidth = 698.478f;

    private float xDistance;
    private float wDistance;

    private RectTransform bar; 

    private void Start()
    {
        xDistance = maxX - minX;
        wDistance = maxWidth - minWidth;
        bar = GetComponent<RectTransform>();
    }
    
    private void Update() {
        
        if(GameManager.Instance.GameState == GameStates.Playing)
        {
            float roundPercentage = GameManager.Instance.RoundTime / GameManager.Instance.gameParams.time;
            Vector3 pos = bar.localPosition;
            bar.localPosition = new Vector3(minX + roundPercentage * xDistance, 688.9108f, pos.z);
            bar.sizeDelta = new Vector2(minWidth + roundPercentage * wDistance, 306.3604f);
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
        bar.position = new Vector3(minX, 688.9108f, -4.693462f);
        bar.sizeDelta = new Vector2(minWidth, 306.3604f);
    }
}
