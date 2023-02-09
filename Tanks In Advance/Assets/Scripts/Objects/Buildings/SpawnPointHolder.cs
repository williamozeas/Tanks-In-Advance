using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointHolder : MonoBehaviour
{
    private List<SpawnPoint> p1SpawnPoints;
    public List<SpawnPoint> P1SpawnPoints => p1SpawnPoints;
    private List<SpawnPoint> p2SpawnPoints;
    public List<SpawnPoint> P2SpawnPoints => p2SpawnPoints;
    // Start is called before the first frame update
    void Awake()
    {
        p1SpawnPoints = new List<SpawnPoint>(GetComponentsInChildren<SpawnPoint>());
        p2SpawnPoints = new List<SpawnPoint>(GetComponentsInChildren<SpawnPoint>());
        
        //Remove each team's spawn points
        p1SpawnPoints.RemoveAll((point => point.playerNum == PlayerNum.Player2));
        p2SpawnPoints.RemoveAll((point => point.playerNum == PlayerNum.Player1));

        //Sort by round
        p1SpawnPoints.Sort(((point1, point2) => point1.round.CompareTo(point2.round)));
        p2SpawnPoints.Sort(((point1, point2) => point1.round.CompareTo(point2.round)));
    }
}
