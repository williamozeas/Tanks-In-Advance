using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapName
{
    EmptyMap = 0,
    TestMap = 1,
}

public class Map : MonoBehaviour
{
    public MapName nameEnum = MapName.TestMap;

    public string name = "Test Map";
    
    private List<SpawnPoint> team1SpawnPoints;
    private List<SpawnPoint> team2SpawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        //On Spawn
        SpawnPointHolder spawnPointHolder = GetComponentInChildren<SpawnPointHolder>();
        team1SpawnPoints = spawnPointHolder.P1SpawnPoints;
        team2SpawnPoints = spawnPointHolder.P2SpawnPoints;
        //Set Lighting, Animation, etc
    }

    public void RemoveMap()
    {
        //animation, lighting, etc
        Destroy(this);
    }

    public SpawnPoint GetSpawnPoint(int team, int round)
    {
        if (team == 1)
        {
            if (round > team1SpawnPoints.Count)
            {
                return team1SpawnPoints[team1SpawnPoints.Count - 1];
            }
            return team1SpawnPoints[round - 1];
        }
        else
        {
            if (round > team2SpawnPoints.Count)
            {
                return team2SpawnPoints[team2SpawnPoints.Count - 1];
            }
            return team2SpawnPoints[round - 1];
        }
    }
    
}
