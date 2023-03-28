using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Bullet
{
    public Mine(Tank source)
    {
        _tank = source;
        _lifespan = 5f;
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        _lifespan = 5f;
    }

    // FixedUpdate called every certain amt of time
    protected override void Update()
    {
        if (_lifespan < 0)
        {
            Explode();
        }
        _lifespan -= Time.deltaTime;
    }

    public void Explode()
    {
        Debug.Log("Explode mine");

        if (!is_ghost)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

            foreach (Collider hit in hits)
            {
                BreakableWall bWall = hit.gameObject.GetComponent<BreakableWall>();
                if (bWall != null)
                {
                    bWall.TakeDamage(power);
                }

                // tank collision detection
                if (hit.transform.parent != null)
                {
                    Tank tank = hit.transform.parent.GetComponent<Tank>();
                    if (tank == _tank && canHitSelf) //To avoid self-destruction.
                    {
                        tank.TakeDamage(power);
                    }
                    else if (tank != null && tank != _tank)
                    {
                        tank.TakeDamage(power);
                    }
                }

                // bullet collision detection
                if (TryGetComponent<Bullet>(out Bullet bullet))
                {
                    // don't explode self
                    if (bullet != this)
                    {
                        if (bullet is Mine)
                            ((Mine)bullet).Explode();
                        else
                            Destroy(bullet);
                    }
                }
            }
        }

        KillSelf();
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (canHitSelf && collision.gameObject != _tank.gameObject)
            Explode();
    }

    protected void OnTriggerExit(Collider collision)
    {
        if (collision.transform.parent == _tank.transform)
            canHitSelf = true;
    }
}
