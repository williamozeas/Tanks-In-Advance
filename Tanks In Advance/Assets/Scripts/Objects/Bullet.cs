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
    public float speed = 5;
    private bool live;
    private int power = 1;
    
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
        this.live = false;
    }

    // FixedUpdate called every certain amt of time
    protected void Update()
    {
        //Debug.Log(velocity);
        base.FixedUpdate();
        if (this._lifespan < 0 || ricochets > 2)
        {
            KillSelf();
        }
        this._lifespan -= Time.deltaTime;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        Collider hit = collision.collider;
        Tank tank; //= new Tank();
        Wall wall;

        if (hit.transform.parent.TryGetComponent<Tank>(out tank))
        {
            
            if (live) //To avoid self-destruction.
            {
                Debug.Log("b");
                tank.TakeDamage(power);
            }

            KillSelf(); //Destroy bullet
        }
        else if (hit.TryGetComponent<Wall>(out wall))
        {
            Debug.Log("boing");
            Ricochet(collision);
            ricochets++;
        }
        else
        {
            KillSelf();
        }

        Debug.Log(collision.collider.name);
    }

    protected void OnCollisionExit(Collision collision)
    {
        live = true; //To ensure that the bullet leaves the owner before being able to kill tanks.

        //If the bullet doesn't interact with some random object, blame this command.
        Physics.IgnoreCollision(
            GetComponent<Collider>(),
            collision.collider,
            false
        );
    }

    protected void Ricochet(Collision coll)
    {
        Vector3 normal = coll.GetContact(0).normal;
        //velocity.x = normal.x * speed;
        //velocity.y = normal.z * speed;
    }

    protected void KillSelf()
    {
        if (_tank.bulletList.Remove(this.gameObject))
        {
            Debug.Log("Remove bullet");
        }
    }
}
