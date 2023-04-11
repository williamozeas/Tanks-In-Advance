using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTank : Tank
{
    protected override TankType type => TankType.laser;

    public Transform turretPos;
    public LayerMask castMask;

    protected override void Update()
    {
        base.Update();

        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        Debug.DrawRay(turretPos.position, adjAim * 50, Color.cyan);
    }

    public override void Shoot(ShootCommand shootCommand)
    {
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0f) return;

        // visuals for shooting
        // vfx.Play();

        shootingCooldown = int.MaxValue;

        StartCoroutine(FireLaser());
    }

    IEnumerator FireLaser()
    {
        disableMovement = true;

        // windup
        yield return new WaitForSeconds(1f);

        float MAX_CAST_DISTANCE = 50;
        int DAMAGE = 10;

        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        RaycastHit rayhit;
        Physics.SphereCast(turretPos.position, 0.1f, adjAim, out rayhit, MAX_CAST_DISTANCE,
            castMask, QueryTriggerInteraction.Collide);

        if (rayhit.collider != null)
        {
            GameObject hit = rayhit.collider.gameObject;

            Debug.Log(hit);

            if (hit.transform.parent != null &&
                hit.transform.parent.TryGetComponent<Tank>(out Tank tank) &&
                tank.Type != TankType.shield)
            {
                tank.TakeDamage(DAMAGE);
            }
            else if (hit.TryGetComponent<BreakableWall>(out BreakableWall wall))
            {
                wall.TakeDamage(DAMAGE);
            }
            else if (hit.TryGetComponent<Bullet>(out Bullet bullet))
            {
                if (bullet is Mine)
                    ((Mine)bullet).Explode();
                else
                    Destroy(bullet);
            }
        }

        // cooldown
        yield return new WaitForSeconds(0.5f);

        shootingCooldown = cooldown;
        disableMovement = false;
    }
}
