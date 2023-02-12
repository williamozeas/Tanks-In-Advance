using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : Command
{
    private Vector2 _angle;
    private Tank _tank;
    //private Bullet _bullet;
    
    public ShootCommand(Vector2 angle, Tank tank, float timestamp)
    {
        _angle = angle;
        _tank = tank;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        Debug.Log("Shooty shoot shoot pew pew");
        //Bullet nbullet = new Bullet(_tank, _angle * 3);
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
