using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Pico = 0,
}

public class Bullet : MovingObject
{
    private Tank _tank;
    private float _lifespan;
    private int ricochets;
    
    public Bullet(Tank source, Vector2 velocity)
    {
        _tank = source;
        this.velocity = velocity;
        this._lifespan = 100.0f;
        this.ricochets = 0;
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        this._lifespan = 100.0f;
        this.ricochets = 0;
    }

    // FixedUpdate called every certain amt of time
    protected void Update()
    {
        Debug.Log(velocity);
        if (this._lifespan < 0 || ricochets > 2)
        {
            Debug.Log("Remove bullet");
            KillSelf();
        }
        this._lifespan -= Time.deltaTime;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        Collider hit = collision.collider;
        Tank tank; //= new Tank();
        Wall wall;

        if (hit.TryGetComponent<Tank>(out tank))
        {
            if (_lifespan < 99.0f) //To avoid self-destruction.
            {

            }

            KillSelf(); //Destroy bullet
        }
        else if (hit.TryGetComponent<Wall>(out wall))
        {
            Debug.Log("boing");
            Ricochet(hit);
            ricochets++;
        }

        Debug.Log(collision.collider.name);
    }

    protected void Ricochet(Collider coll)
    {

    }

    protected void KillSelf()
    {

    }
}
