using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : Singleton<TankManager>
{
    public GameObject BlueTankPrefab;
    public GameObject RedTankPrefab;

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
        SpawnPoint spawnPoint1 =
            GameManager.Instance.mapSpawner.CurrentMap.GetSpawnPoint(PlayerNum.Player1, round.number);
        Tank p1Tank = SpawnNewTank(TankType.basic, PlayerNum.Player1, spawnPoint1.transform.position, 
            Quaternion.Euler(0, 0, 0));
        p1Tanks.Add(p1Tank);
        
        SpawnPoint spawnPoint2 =
            GameManager.Instance.mapSpawner.CurrentMap.GetSpawnPoint(PlayerNum.Player2, round.number);
        Tank p2Tank = SpawnNewTank(TankType.basic, PlayerNum.Player2, spawnPoint2.transform.position, 
            Quaternion.Euler(0, 180, 0));
        p2Tanks.Add(p2Tank);
    }

    public Tank SpawnNewTank(TankType type, PlayerNum playerNum, Vector3 position, Quaternion rotation)
    {
        GameObject tankPrefab = GameManager.Instance.TankList.GetTank(playerNum, type);
        Tank newTank = Instantiate(tankPrefab, position, rotation, transform).GetComponent<Tank>();
        newTank.AssignToTeam(playerNum);
        return newTank;
    }
}
