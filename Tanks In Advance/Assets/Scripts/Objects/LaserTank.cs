using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserTank : Tank
{
    protected override TankType type => TankType.laser;

    public float windupTime = 1f;
    public float cooldownTime = 1.5f;

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
        if (Physics.Raycast(turretPos.position, adjAim, out hit, 1000f, castMask) &&
            Physics.Raycast(hit.point, -adjAim, out reverseHit, 1000f, castMask))
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
        }
    }

    public override void Shoot(ShootCommand shootCommand)
    {
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0f) return;


        shootingCooldown = int.MaxValue;

        StartCoroutine(FireLaser());
    }

    IEnumerator FireLaser()
    {
        disableMovement = true;

        // windup
        float timeElapsed = 0;
        float startWidth = laserLine.widthCurve[0].value;
        while (timeElapsed < windupTime)
        {
            laserLine.widthCurve = AnimationCurve.Constant(0, 1, startWidth * (1 - (timeElapsed / windupTime)));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        float MAX_CAST_DISTANCE = 50;
        int DAMAGE = 10;

        Vector3 adjAim = new Vector3(aim.x, 0, aim.y);
        List<RaycastHit> rayhits = Physics.SphereCastAll(turretPos.position, 0.1f, adjAim, MAX_CAST_DISTANCE,
            castMask, QueryTriggerInteraction.Collide).ToList();
        
        float distance = rayhits.Where((hit) =>
        {
            if (hit.transform.TryGetComponent<ShieldTank>(out ShieldTank tank))
            {
                return true;
            }
            if (hit.transform.TryGetComponent<Wall>(out Wall wall))
            {
                return wall.blocksLaser;
            }
            return false;

        }).Min(hit => hit.distance);

        foreach (RaycastHit rayhit in rayhits)
        {
            if (rayhit.distance > distance) continue;
            GameObject hit = rayhit.transform.gameObject;

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

        // visuals for shooting
        ShootVfx.SetInt("PlayerNum", (int)ownerNum);
        ShootVfx.SetFloat("Distance", distance * 0.52f);
        ShootVfx.Play();

        // cooldown
        timeElapsed = 0;
        yield return new WaitForSeconds(cooldownTime);
        disableMovement = false;
        float remainingCooldown = 2f;
        while (timeElapsed < remainingCooldown)
        {
            laserLine.widthCurve = AnimationCurve.Constant(0, 1, startWidth * (timeElapsed / remainingCooldown));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        laserLine.widthCurve = AnimationCurve.Constant(0, 1, startWidth);

        shootingCooldown = cooldown;
    }

    public override void Die()
    {
        base.Die();
        laserLine.enabled = false;
    }

    public override void Ghost()
    {
        base.Ghost();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        MaterialMod.SetOpacity(0.2f, GetComponent<MeshRenderer>(), propBlock);
    }
}
