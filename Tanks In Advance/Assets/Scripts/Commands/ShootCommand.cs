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

        GameObject bullet = Object.Instantiate(
            _tank.bulletPrefab,
            _tank.rb.position + new Vector3(_angle.x, 0, _angle.y) * 1f,
            Quaternion.Euler(_angle.x, 0, _angle.y)
        );

        Bullet bulletBullet = bullet.GetComponent<Bullet>();
        bulletBullet.Init(_tank, _angle * 5);

        Physics.IgnoreCollision(
            bulletBullet.GetComponent<Collider>(),
            _tank.GetComponentInChildren<Collider>(),
            true
        );

        _tank.bulletList.Add(bullet);
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
