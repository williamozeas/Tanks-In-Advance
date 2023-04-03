using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAimCommand : Command
{
    private Vector2 _angle;
    private Tank _tank;
    
    public SetAimCommand(Vector2 angle, Tank tank, float timestamp)
    {
        _angle = angle;
        _tank = tank;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        if (_tank != null)
            _tank.SetAim(_angle);
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
