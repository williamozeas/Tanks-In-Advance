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
        //Shield Tank can't shoot
        if(_tank.Type == TankType.shield) return;
        //The tank does nothing if shooting has not yet cooled down.
        if (_tank.shootingCooldown > 0.5f) return;

        _tank.Shoot(); //visuals

        //Tank must wait to shoot again.
        if (_tank.shootingCooldown < 0.1f) 
        {
            _tank.shootingCooldown += 0.6f; //Rapid barrage after waiting
        }
        else                                
        {
            _tank.shootingCooldown += 0.8f; //Space out following shots.
        }
        //_tank.shootingCooldown += 0.2f * _tank.rb.velocity.magnitude; //Potential


        GameObject bullet = Object.Instantiate(
            _tank.bulletPrefab,
            _tank.rb.position + new Vector3(_angle.x, 0, _angle.y) * 1f,
            Quaternion.Euler(_angle.x, 0, _angle.y)
        );

        Bullet bulletBullet = bullet.GetComponent<Bullet>();
        bulletBullet.Init(_tank, _angle);

        _tank.bulletList.Add(bullet);
    }

    public override string ToString()
    {
        return _tank.name + ": Shoot at angle " + _angle;
    }
}
