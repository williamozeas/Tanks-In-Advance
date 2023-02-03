using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Any moving object with a stored velocity. This is separated out to enable command pattern.
 */
public abstract class MovingObject : MonoBehaviour
{
    protected Vector2 velocity;
    public Vector2 Velocity => velocity;
    public Rigidbody rb;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        Debug.Log(transform.position);
    }

    // FixedUpdate called every certain amt of time
    // used here to ensure variable framerate won't mess up our replay
    protected virtual void FixedUpdate()
    {
        rb.velocity = new Vector3(velocity.x, 0, velocity.y);
    }
    
    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }
}
