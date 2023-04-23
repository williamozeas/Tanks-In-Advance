using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTank : Tank
{
    protected override TankType type => TankType.mine;

    public override void Shoot(ShootCommand shootCommand)
    {
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0f) return;

        // visuals for shooting
        // vfx.Play();

        shootingCooldown = cooldown;

        GameObject mine = Instantiate(bulletPrefab, rb.transform);
        mine.transform.SetParent(null);
        mine.transform.position += Vector3.down * 0.5f;

        Mine mineC = mine.GetComponent<Mine>();
        mineC.Init(this, shootCommand._angle);

        bulletList.Add(mine);
    }
}
