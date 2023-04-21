using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    private Map currentMap;

    public Map CurrentMap => currentMap;
    // Awake is called before Start
    void Awake()
    {
        GameManager.Instance.mapSpawner = this;
    }

    void Start()
    {
        //DEBUG
        // SpawnMap(MapName.Level4);
    }

    private void OnEnable()
    {
        GameManager.OnMainMenu += OnMainMenu;
    }

    private void OnDisable()
    {
        GameManager.OnMainMenu -= OnMainMenu;
    }

    private void OnMainMenu()
    {
        if (currentMap != null)
        {
            currentMap.RemoveMap();
        }
    }

    public void SpawnMap(MapName name)
    {
        currentMap = Instantiate(GameManager.Instance.MapList.GetMap(name)).GetComponent<Map>();
    }
}
