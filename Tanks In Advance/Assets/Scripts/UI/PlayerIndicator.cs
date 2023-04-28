using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIndicator : MonoBehaviour
{
    private Image _chevron;
    private Tank _tank;

    [SerializeField] [ColorUsageAttribute(true,true)] private Color blue;
    [SerializeField] [ColorUsageAttribute(true,true)] private Color red;

    private Vector3 _startPos;
    private float _time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _chevron = GetComponent<Image>();
        _startPos = transform.localPosition;
        if (!_tank || !_tank.Owner)
        {
            return;
        }
        if (_tank.Owner.PlayerNumber == PlayerNum.Player1)
        {
            _chevron.color = blue;
        }
        else
        {
            _chevron.color = red;
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        transform.localPosition = new Vector3(_startPos.x, _startPos.y + Mathf.Sin(_time * Mathf.PI) * 0.5f + 0.5f, _startPos.z);
        if (!_tank.Alive)
        {
            _chevron.color = Color.white;
        }
        else
        {
            if (_tank.Owner.PlayerNumber == PlayerNum.Player1)
            {
                _chevron.color = blue;
            }
            else
            {
                _chevron.color = red;
            }
        }
    }

    public void SetTank(Tank newTank)
    {
        _tank = newTank;
    }

    public void SetState(bool active)
    {
        _chevron.enabled = active;
    }
    
}
