using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTank : Tank
{
    protected override TankType type => TankType.laser;

    public Transform turretPos;
    public LayerMask castMask;
    [SerializeField] LineRenderer laserLine;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material pinkMat;

    protected override void Start()
    {
        base.Start();
        if (ownerNum == PlayerNum.Player1)
        {
            laserLine.sharedMaterial = blueMat;
        } 
        else if (ownerNum == PlayerNum.Player2)
        {
            laserLine.sharedMaterial = pinkMat;
        }
    }

    protected override void Update()
    {
        base.Update();
        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        laserLine.SetPosition(0, turretPos.position);
        RaycastHit hit;
        RaycastHit reverseHit; //for raycasting from inside of a wall
        if (Physics.Raycast(turretPos.position, adjAim, out hit, 1000f) &&
            Physics.Raycast(hit.point, -adjAim, out reverseHit, 1000f))
        {
            Vector3 hitPoint;
            float diff = hit.distance - reverseHit.distance;
            if(diff < 0)
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = turretPos.position + adjAim * diff;
            }
            laserLine.SetPosition(1, hitPoint);
            
            Debug.DrawRay(turretPos.position, adjAim * hit.distance, Color.cyan);
            Debug.DrawRay(hit.point, -adjAim * reverseHit.distance, Color.red);
            Debug.Log(reverseHit.collider.gameObject.name);
        }
        // Physics.queriesHitBackfaces = false;
    }

    public override void Shoot(ShootCommand shootCommand)
    {
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0f) return;

        // visuals for shooting
        // ShootVfx.Play();
        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        Debug.DrawRay(turretPos.position, adjAim * 50, Color.cyan, 500f);

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
