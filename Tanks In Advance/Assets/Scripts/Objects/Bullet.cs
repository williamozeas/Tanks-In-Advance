using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MovingObject
{
    private Tank _tank;
    
    public Bullet(Tank source, Vector2 velocity)
    {
        _tank = source;
        this.velocity = velocity;
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // FixedUpdate called every certain amt of time
    protected override void FixedUpdate()
    {
        
    }
}
