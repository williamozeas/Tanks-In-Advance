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
        _tank.Shoot(); //visuals
        Debug.Log("Shooty shoot shoot pew pew");
        //Quaternion q = Quaternion.Euler(new Vector3(_angle.x, 0, _angle.y));
        //_tank.bulletPrefab.GetComponent<Bullet>().getShot(_tank, _angle * 3);
        GameObject bullet = Object.Instantiate(_tank.bulletPrefab, _tank.rb.transform);
        bullet.GetComponent<Bullet>().SetVelocity(_angle * 3);
        _tank.bulletList.Add(bullet);
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
