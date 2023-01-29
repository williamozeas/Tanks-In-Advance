using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Tanks will recieve input from the player and store that input in a list of commands, which will then
 * be replayed on subsequent rounds.
 */
public class Tank : MovingObject
{
    
    [Header("Stats")] 
    public float speed = 1.0f;

    protected List<Command> commandList = new List<Command>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public void AddCommand(Command newCommand)
    {
        commandList.Add(newCommand);
    }

    public void Shoot()
    {
        
    }
}
