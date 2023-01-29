using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : Singleton<TankManager>
{
    public GameObject TankPrefab;

    private List<Tank> p1Tanks = new List<Tank>();
    private List<Tank> p2Tanks = new List<Tank>();
    
    private void OnEnable()
    {
        GameManager.OnRoundStart += OnRoundStart;
    }
    private void OnDisable()
    {
        GameManager.OnRoundStart -= OnRoundStart;
    }

    private void OnRoundStart(Round round)
    {
        Tank p1Tank = SpawnNewTank(new Vector3(-2, (round.number - 1) * 2, 0.5f), Quaternion.Euler(0, 0, 0), PlayerNum.Player1);
        p1Tanks.Add(p1Tank);
        GameManager.Instance.Players[0].SetCurrentTank(p1Tank);
        
        Tank p2Tank = SpawnNewTank(new Vector3(2, (round.number - 1) * 2, 0.5f), Quaternion.Euler(0, 180, 0), PlayerNum.Player2);
        p2Tanks.Add(p2Tank);
        GameManager.Instance.Players[1].SetCurrentTank(p2Tank);
    }

    public Tank SpawnNewTank(Vector3 position, Quaternion rotation, PlayerNum playerNum)
    {
        //TODO: Spawn new tank
        Tank newTank = Instantiate(TankPrefab, position, rotation, transform).GetComponent<Tank>();
        newTank.owner = playerNum;
        return newTank;
    }
}
