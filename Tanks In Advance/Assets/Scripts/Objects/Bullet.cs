using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Pico = 0,
}

public class Bullet : MonoBehaviour
{
    private Tank _tank;
    private float _lifespan;
    private int ricochets;
    public float speed = 5;
    private bool live;
    private int power = 1;
    private Vector2 velocity;
    
    //called on creation
    public void Init(Tank source, Vector2 angle)
    {
        _tank = source;
        this.velocity = angle.normalized * speed;
        _lifespan = 100.0f;
        ricochets = 0;
    }
    
    // Start is called before the first frame update
    protected void Start()
    {
        this._lifespan = 100.0f;
        this.ricochets = 0;
        this.live = false;
    }

    // Update called every frame
    protected void Update()
    {
        if (this._lifespan < 0 || ricochets > 2)
        {
            KillSelf();
        }
        this._lifespan -= Time.deltaTime;
    }

    protected void FixedUpdate()
    {
        Vector3 actualVelocity = new Vector3(velocity.x, 0, velocity.y);
        transform.Translate(actualVelocity * Time.fixedDeltaTime);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        Collider hit = collision.collider;
        Tank tank;
        Wall wall;
        
        Debug.Log("COLLIDED");
        
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tanks") && hit.transform.parent.TryGetComponent<Tank>(out tank))
        {
            
            if (tank == _tank && live) //To avoid self-destruction.
            {
                tank.TakeDamage(power);
            } else if (tank != _tank)
            {
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tanks"))
        {
            live = true; //To ensure that the bullet leaves the owner before being able to kill its owner.
        }
        
    }

    protected void Ricochet(Collision coll)
    {
        Vector3 normal = coll.GetContact(0).normal;
        Vector2 normal2 = new Vector2(normal.x, normal.z);
        Vector2 incident = velocity;
        Vector2 normalComponent = Vector2.Dot(normal2, incident) * normal2;
        velocity = incident - 2 * normalComponent;
    }

    protected void KillSelf()
    {
        _tank.bulletList.Remove(this.gameObject);
        //Destroy animation
        Destroy(gameObject);
    }
}
