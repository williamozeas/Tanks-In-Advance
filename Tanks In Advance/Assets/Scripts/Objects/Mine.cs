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

    protected void Explode()
    {
        Debug.Log("Explode mine");

        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

        foreach (Collider hit in hits)
        {
            BreakableWall bWall = hit.gameObject.GetComponent<BreakableWall>();
            if (bWall != null)
            {
                Debug.Log("Got here");
                bWall.Die();
            }

            Tank tank = hit.gameObject.GetComponent<Tank>();
            if (tank == _tank && canHitSelf) //To avoid self-destruction.
            {
                tank.TakeDamage(power);
            }
            else if (tank != null && tank != _tank)
            {
                tank.TakeDamage(power);
            }
        }

        KillSelf();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (canHitSelf)
            Explode();
    }
}
