using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBlock : BreakableWall
{
    public UnityEvent OnHit;
    public float delay = 0f;
    public bool triggered = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullets"))
        {
            Die();
        }
    }

    public override void Die()
    {
        if (!triggered)
        {
            triggered = true;
            Invoke("Call", delay);
            base.Die();
        }
    }

    void Call()
    {
        OnHit?.Invoke();
        triggered = false;
    }
}
