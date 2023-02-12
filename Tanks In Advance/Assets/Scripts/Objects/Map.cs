using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    
    public GameObject wallsHolder;
    
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
        
        //Animation
        StartCoroutine(AddMapAnim());
    }

    public IEnumerator AddMapAnim()
    {
        List < Wall > walls = new List<Wall>(wallsHolder.GetComponentsInChildren<Wall>());
        walls.Sort((wall1, wall2) =>
        {
            float wall1Heuristic = -wall1.transform.position.z + wall1.transform.position.x;
            float wall2Heuristic = -wall2.transform.position.z + wall2.transform.position.x;
            return wall1Heuristic.CompareTo(wall2Heuristic);
        });
        foreach(Wall wall in walls)
        {
            wall.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach(Wall wall in walls)
        {
            wall.GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(wall.OnCreate());
            yield return new WaitForSeconds(0.01f);
        }

    }

    public void RemoveMap()
    {
        //animation, lighting, etc
        Destroy(this);
    }

    public SpawnPoint GetSpawnPoint(PlayerNum team, int round)
    {
        if (team == PlayerNum.Player1)
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
