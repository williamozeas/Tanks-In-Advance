using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to store Map prefabs
[CreateAssetMenu(fileName = "MapList", menuName = "ScriptableObjects/MapList", order = 1)] [Serializable]
public class MapList : ScriptableObject
{
    [SerializeField] private List<GameObject> mapList;
    private Dictionary<MapName, GameObject> maps = new Dictionary<MapName, GameObject>();

    //you can't serialize dictionaries so I'm populating this one from a list
    //must be called on game start
    public void Init()
    {
        maps.Clear();
        foreach (var mapObj in mapList)
        {
            Map map = mapObj.GetComponent<Map>();
            maps.Add(map.nameEnum, mapObj);
        }
    }

    public GameObject GetMap(MapName name)
    {
        return maps[name];
    }
}
