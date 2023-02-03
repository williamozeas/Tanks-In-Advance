using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : Command
{
    private Vector2 _angle;
    private Tank _tank;
    private Bullet _bullet;
    
    public ShootCommand(Vector2 angle, Bullet bullet, Tank tank, float timestamp)
    {
        _angle = angle;
        _tank = tank;
        _bullet = bullet;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        Debug.Log(ToString());
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
