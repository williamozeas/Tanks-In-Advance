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
    protected Tank _tank;
    protected float _lifespan;
    private int ricochets;
    public float speed = 5;
    protected bool live;
    public int power = 1;
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
    protected virtual void Start()
    {
        this._lifespan = 100.0f;
        this.ricochets = 0;
        this.live = false;
    }

    // Update called every frame
    protected virtual void Update()
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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Collider hit = collision.collider;
        Tank tank;
        Wall wall;


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
            Ricochet(collision);
            ricochets++;
        }
        else
        {
            KillSelf();
        }

    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tanks"))
        {
            live = true; //To ensure that the bullet leaves the owner before being able to kill its owner.
            // _tank.SetCollisions(GetComponent<Collider>(), true);
        }
        
    }

    protected void Ricochet(Collision coll, bool quantizeNormal = true)
    {
        float tolerance = 0.01f;
        
        Vector2 incident = velocity;
        Vector3 normal = coll.GetContact(0).normal;
        Vector2 normal2;

        //quantize weird normals
        if (quantizeNormal && (normal.x != 0 && normal.z != 0))
        {
            //quantize to 45 degree angles
            normal2 = new Vector2(Math.Sign(normal.x), Math.Sign(normal.z)).normalized;
            Debug.Log("quantized " + normal + " to " + normal2);
        }
        else
        {
            normal2 = new Vector2(normal.x, normal.z);
            
            //quantize perpendicular collisions
            float dot = Vector2.Dot(normal2, incident);
            if (quantizeNormal && (Mathf.Abs(dot) < tolerance))
            {
                //quantize to 45 degree angles
                normal2 = new Vector2(Math.Sign(normal.x - incident.x), Math.Sign(normal.z - incident.y)).normalized;
                Debug.Log("quantized " + normal + " to " + normal2);
            }
            Debug.Log("did not quantize " + normal);
        }
        Vector2 normalComponent = Vector2.Dot(normal2, incident) * normal2;
        velocity = incident - 2 * normalComponent;
    }

    protected void KillSelf()
    {
        if (_tank.bulletList.Contains(gameObject))
            _tank.bulletList.Remove(gameObject);

        //Destroy animation
        Destroy(gameObject);
    }
}
