using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


//Class to store Tank prefabs
[CreateAssetMenu(fileName = "TankList", menuName = "ScriptableObjects/TankList", order = 1)]
public class TankList : ScriptableObject
{
    [SerializeField] private List<GameObject> redTankList;
    private Dictionary<TankType, GameObject> redTanks = new Dictionary<TankType, GameObject>();
    
    [SerializeField] private List<GameObject> blueTankList;
    private Dictionary<TankType, GameObject> blueTanks = new Dictionary<TankType, GameObject>();

    //you can't serialize dictionaries so I'm populating this one from a list
    //must be called on game start
    public void Init()
    {
        redTanks.Clear();
        foreach (var tankObj in redTankList)
        {
            Tank tank = tankObj.GetComponent<Tank>();
            redTanks.Add(tank.Type, tankObj);
        }
        
        blueTanks.Clear();
        foreach (var tankObj in blueTankList)
        {
            Tank tank = tankObj.GetComponent<Tank>();
            blueTanks.Add(tank.Type, tankObj);
        }
    }
    
    public GameObject GetTank(PlayerNum num, TankType name)
    {
        if (num == PlayerNum.Player1)
        {
            return blueTanks[name];
        }
        return redTanks[name];
    }
}
