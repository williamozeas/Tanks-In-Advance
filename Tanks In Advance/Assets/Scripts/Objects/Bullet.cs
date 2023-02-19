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
    
    public Bullet(Tank source, Vector2 velocity)
    {
        _tank = source;
        this.velocity = velocity;
        this._lifespan = 100.0f;
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        this._lifespan = 100.0f;
    }

    // FixedUpdate called every certain amt of time
    protected void Update()
    {
        Debug.Log(velocity);
        if (this._lifespan < 0)
        {
            Debug.Log("Remove bullet");
        }
        this._lifespan -= Time.deltaTime;
    }
}
