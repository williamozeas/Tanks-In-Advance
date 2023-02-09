using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to store Tank prefabs
[CreateAssetMenu(fileName = "TankList", menuName = "ScriptableObjects/TankList", order = 1)]
public class TankList : ScriptableObject
{
    [SerializeField] private List<GameObject> tankList;
    private Dictionary<TankType, GameObject> tanks = new Dictionary<TankType, GameObject>();

    //you can't serialize dictionaries so I'm populating this one from a list
    public void Awake()
    {
        foreach (var tankObj in tankList)
        {
            Tank tank = tankObj.GetComponent<Tank>();
            tanks.Add(tank.Type, tankObj);
        }
    }
    
    public GameObject GetTank(TankType name)
    {
        return tanks[name];
    }
}
