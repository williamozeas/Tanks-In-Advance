using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tanks will recieve input from the player and store that input in a list of commands, which will then
 * be replayed on subsequent rounds.
 */
public class Tank : MovingObject
{
    public PlayerNum owner;
    
    [Header("Stats")] 
    public float speed = 1.0f;

    private Vector3 _startLocation = Vector3.zero;

    protected List<Command> commandList = new List<Command>();
    public bool IsRecorded => commandList.Count > 0;
    
    // Start will be executed when the tank spawns in
    protected override void Start()
    {
        base.Start();
        _startLocation = transform.position;
    }
    
    //Subscribe to events
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

    public void OnRoundStart(Round round)
    {
        if (IsRecorded)
        {
            transform.position = _startLocation;
            StartCoroutine(Replay());
        }
        else
        {
            _startLocation = transform.position;
        }
    }
    
    public void OnRoundEnd()
    {
        if (IsRecorded)
        {
            transform.position = _startLocation;
        }
    }

    public void AddCommand(Command newCommand)
    {
        commandList.Add(newCommand);
    }

    public void Shoot()
    {
        
    }

    //Requires commandList to be in order by timestamp to work properly
    public IEnumerator Replay()
    {
        var enumerator = commandList.GetEnumerator();
        // for (int i = 0; i < commandList.Count; i++)
        while(enumerator.MoveNext())
        {
            Command nextCommand = enumerator.Current;
            //wait for time to be at correct timestamp
            while (GameManager.Instance.RoundTime < nextCommand.Timestamp)
            {
                yield return new WaitForFixedUpdate();
            }
            nextCommand.Execute();
            Debug.Log("Set Velocity to " + nextCommand.ToString());
        }
    }
}
