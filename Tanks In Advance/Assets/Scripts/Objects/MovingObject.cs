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
    private bool initializeVelocity = true;
    
    public Vector2 Velocity => velocity;
    public Rigidbody rb;

    private float rotation;
    private Coroutine rotationCoroutine;
    
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
        // Debug.Log(transform.position);
    }

    // FixedUpdate called every certain amt of time
    // used here to ensure variable framerate won't mess up our replay
    protected virtual void FixedUpdate()
    {
        rb.velocity = new Vector3(velocity.x, 0, velocity.y); //this is needed to prevent bouncing off of walls
    }
    
    public void SetVelocity(Vector2 newVelocity)
    {
        if (velocity.Equals(newVelocity))
        {
            return;
        }
        
        // set velocity
        velocity = newVelocity;
        rb.velocity = new Vector3(velocity.x, 0, velocity.y);

        if (initializeVelocity)
        {
            float newRotation = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            rb.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,1,0));
            initializeVelocity = false;
            return;
        }
        
        // set rotation
        if (velocity.x != 0 || velocity.y != 0)
        {
            float newRotation = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
                rotationCoroutine = null;
            }
            rotationCoroutine = StartCoroutine(EaseRotation(rotation, newRotation, 0.5f));
        }
    }

    public void Reset()
    {
        SetVelocity(Vector2.zero);
        StopCoroutine(rotationCoroutine);
        rotationCoroutine = null;
        rotation = 0;
        rb.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,1,0));
    }

    IEnumerator EaseRotation(float start, float end, float duration)
    {
        // We want angles to be in the range in (-180, 180]
        if (start <= -180)
        {
            start += 360;
        }
        if (start > 180)
        {
            start -= 360;
        }
        if (end <= -180)
        {
            end += 360;
        }
        if (end > 180)
        {
            end -= 360;
        }
        
        // we want to take the shorter turn
        float r1 = Mathf.Abs(end - start);
        float r2 = Mathf.Abs(end - 360 - start);
        float r3 = Mathf.Abs(end - (start - 360));
        if (r2 < r1 && r2 < r3)
        {
            end -= 360;
        }

        if (r3 < r1 && r3 < r2)
        {
            start -= 360;
        }
        
        Debug.Assert(Mathf.Abs(start - end) <= 180);
        
        
        // If the rotation is more than 100 degrees, the turn is too large and it doesn't look good
        // I will not play any animation if it does happen
        if (Mathf.Abs(start - end) > 100)
        {
            rotation = end;
            rb.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,1,0));
            rotationCoroutine = null;
            yield break;
        }

            
        // Rotation starts
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            rotation = EasingFunction.EaseOutCubic(start, end, timeElapsed / duration);
            rb.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,-1,0));
            yield return new WaitForSeconds(0.01f);
            timeElapsed += 0.01f;
        }
        
        rotation = end;
        rb.rotation = Quaternion.AngleAxis(rotation, new Vector3(0,1,0));

        rotationCoroutine = null;
    }
}
