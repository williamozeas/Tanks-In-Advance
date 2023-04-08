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

    [HideInInspector] public Dictionary<TankType, int> _p1AvailableTanks = new Dictionary<TankType, int>();
    [HideInInspector] public Dictionary<TankType, int> _p2AvailableTanks = new Dictionary<TankType, int>();

    public TankType P1selectedType = TankType.basic;
    public TankType P2selectedType = TankType.basic;

    public override void Awake()
    {
        base.Awake();
        foreach (var tankEntry in GameManager.Instance.tankChoices)
        {
            _p1AvailableTanks.Add(tankEntry.type, tankEntry.num);
            _p2AvailableTanks.Add(tankEntry.type, tankEntry.num);
        }
    }
    
    private void OnEnable()
    {
        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnRoundEnd += OnRoundEnd;
    }
    private void OnDisable()
    {
        GameManager.OnRoundStart -= OnRoundStart;
        GameManager.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundEnd()
    {
        P1selectedType = TankType.basic;
        P2selectedType = TankType.basic;
    }
 
    private void OnRoundStart(Round round)
    {
        //TODO: Replace tanktype.basic + (rn % 3) with p1selectedType to regain normal function.

        SpawnPoint spawnPoint1 =
            GameManager.Instance.mapSpawner.CurrentMap.GetSpawnPoint(PlayerNum.Player1, round.number);
        Tank p1Tank = SpawnNewTank(TankType.basic + ((round.number - 1) % 3), PlayerNum.Player1, spawnPoint1.transform.position, 
            Quaternion.Euler(0, 0, 0));
        p1Tanks.Add(p1Tank);
        
        SpawnPoint spawnPoint2 =
            GameManager.Instance.mapSpawner.CurrentMap.GetSpawnPoint(PlayerNum.Player2, round.number);
        Tank p2Tank = SpawnNewTank(TankType.basic + ((round.number - 1) % 3), PlayerNum.Player2, spawnPoint2.transform.position, 
            Quaternion.Euler(0, 180, 0));
        p2Tanks.Add(p2Tank);
    }

    public Tank SpawnNewTank(TankType type, PlayerNum playerNum, Vector3 position, Quaternion rotation)
    {
        if (playerNum == PlayerNum.Player1)
        {
            if (_p1AvailableTanks[type] <= 0)
            {
                Debug.LogError("Tank spawned when player should be out of that type of tank!");
            }
            _p1AvailableTanks[type]--;
        }
        else if (playerNum == PlayerNum.Player2)
        {
            if (_p1AvailableTanks[type] <= 0)
            {
                Debug.LogError("Tank spawned when player should be out of that type of tank!");
            }
            _p2AvailableTanks[type]--;
        }
        
        GameObject tankPrefab = GameManager.Instance.TankList.GetTank(playerNum, type);
        Tank newTank = Instantiate(tankPrefab, position, rotation, transform).GetComponent<Tank>();
        newTank.AssignToTeam(playerNum);
        return newTank;
    }
}
