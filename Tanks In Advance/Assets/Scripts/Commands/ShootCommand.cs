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

        GameObject bullet = Object.Instantiate(
            _tank.bulletPrefab,
            _tank.rb.position + new Vector3(_angle.x, 0, _angle.y) * 1f,
            Quaternion.Euler(_angle.x, 0, _angle.y),
            _tank.rb.transform
        );

        Bullet bulletBullet = bullet.GetComponent<Bullet>();
        bulletBullet.SetVelocity(_angle * 5);

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
