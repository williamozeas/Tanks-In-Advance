using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundIndicator : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Sprite fullSprite; 
    [SerializeField] private Sprite emptySprite;

    private Image myImage;

    private void Start()
    {
        myImage = GetComponent<Image>();
        myImage.sprite = emptySprite;
    }

    private void Update()
    {
        if (GameManager.Instance.RoundNumber >= index)
        {
            myImage.sprite = fullSprite;
        }
        else
        {
            myImage.sprite = emptySprite;
        }
    }
}
