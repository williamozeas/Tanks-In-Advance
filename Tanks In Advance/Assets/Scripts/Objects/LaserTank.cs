using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTank : Tank
{
    protected override TankType type => TankType.laser;

    public Transform turretPos;

    protected override void Update()
    {
        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        Debug.DrawRay(turretPos.position, adjAim * 20, Color.cyan);
    }

    public override void Shoot(ShootCommand shootCommand)
    {
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0f) return;

        // visuals for shooting
        // vfx.Play();

        shootingCooldown = cooldown;

        
    }
}
