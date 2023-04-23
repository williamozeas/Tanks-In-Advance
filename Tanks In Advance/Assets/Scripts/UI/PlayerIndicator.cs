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
    
    // Start is called before the first frame update
    void Start()
    {
        _chevron = GetComponent<Image>();
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

    public void SetTank(Tank newTank)
    {
        _tank = newTank;
    }

    public void SetState(bool active)
    {
        _chevron.enabled = active;
    }
    
}
