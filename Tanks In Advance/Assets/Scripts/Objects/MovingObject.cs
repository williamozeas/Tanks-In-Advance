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
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // FixedUpdate called every certain amt of time
    // used here to ensure variable framerate won't mess up our replay
    protected virtual void FixedUpdate()
    {
        transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.fixedDeltaTime;
    }
    
    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }
}
