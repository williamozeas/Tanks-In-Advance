using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MapName
{
    EmptyMap = 0,
    TestMap = 1,
    MenuMap = 2,
    Level1 = 3,
    Level2 = 4,
    Level3 = 5,
    Level4 = 6,
    Level5 = 7
}

public class Map : MonoBehaviour
{
    public MapName nameEnum = MapName.TestMap;
    public string name = "Test Map";

    [SerializeField] [ColorUsage(true, true)] private Color outlineColor = Color.black;
    
    public GameObject wallsHolder;
    [SerializeField] private Material outlineMat;
    private WinCircle winCircle;
    
    private List<SpawnPoint> team1SpawnPoints;
    private List<SpawnPoint> team2SpawnPoints;

    private void Awake()
    {
        winCircle = GetComponentInChildren<WinCircle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //On Spawn
        SpawnPointHolder spawnPointHolder = GetComponentInChildren<SpawnPointHolder>();
        team1SpawnPoints = spawnPointHolder.P1SpawnPoints;
        team2SpawnPoints = spawnPointHolder.P2SpawnPoints;
        
        //Set Lighting, Animation, etc
        if (outlineColor != Color.black)
        {
            outlineMat.color = outlineColor;
        }
        
        //Animation
        StartCoroutine(AddMapAnim());
    }

    public IEnumerator AddMapAnim()
    {
        GameManager.Instance.inputLocked = true;
        if (winCircle)
        {
            StartCoroutine(winCircle.IntroAnim(3f));
        }

        List < Wall > walls = new List<Wall>(wallsHolder.GetComponentsInChildren<Wall>());
        float interval = 2f / walls.Count;
        
        walls.Sort((wall1, wall2) =>
        {
            float wall1Heuristic = -wall1.transform.position.z + wall1.transform.position.x;
            float wall2Heuristic = -wall2.transform.position.z + wall2.transform.position.x;
            return wall1Heuristic.CompareTo(wall2Heuristic);
        });
        foreach(Wall wall in walls)
        {
            wall.GetComponent<MeshRenderer>().enabled = false;

            Canvas canvas = wall.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.enabled = false;
        }
        foreach(Wall wall in walls)
        {
            wall.GetComponent<MeshRenderer>().enabled = true;

            Canvas canvas = wall.GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.enabled = true;

            StartCoroutine(wall.OnCreate());
            yield return new WaitForSeconds(interval);
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.inputLocked = false;

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

    public int GetCircleTotal()
    {
        return winCircle.numTanksP1 - winCircle.numTanksP2;
    }

    public PlayerNum GetWinner()
    {
        PlayerNum surviving = TankManager.Instance.CheckForVictory();
        if (surviving != PlayerNum.Neither)
        {
            return surviving;
        }
        
        if (winCircle.numTanksP1 > winCircle.numTanksP2)
        {
            return PlayerNum.Player1;
        }
        else if(winCircle.numTanksP1 < winCircle.numTanksP2)
        {
            return PlayerNum.Player2;
        }
        else
        {
            return PlayerNum.Neither;
        }
    }
    
}
