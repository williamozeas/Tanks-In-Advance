using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCommand : Command
{
    private Tank _tank;
    
    public MineCommand(Tank tank, float timestamp)
    {
        _tank = tank;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        Debug.Log("Mine");
        GameObject mine = Object.Instantiate(_tank.minePrefab, _tank.rb.transform);
        mine.transform.SetParent(null);
        mine.transform.position += Vector3.down * 0.5f;
        _tank.mineList.Add(mine);
    }

    public override string ToString()
    {
        return _tank.name + ": Laid mine";
    }
}
