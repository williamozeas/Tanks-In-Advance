using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private Tank _tank;
    private float _lifespan;
    
    public Mine(Tank source)
    {
        _tank = source;
        _lifespan = 5f;
    }
    
    // Start is called before the first frame update
    protected void Start()
    {
        _lifespan = 5f;
    }

    // FixedUpdate called every certain amt of time
    protected void Update()
    {
        if (_lifespan < 0)
        {
            Explode();
        }
        _lifespan -= Time.deltaTime;
    }

    protected void Explode()
    {
        Debug.Log("Explode mine");

        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

        foreach (Collider hit in hits)
        {
            BreakableWall bWall = hit.gameObject.GetComponent<BreakableWall>();
            if (bWall != null)
            {
                Debug.Log("Got here");
                bWall.Die();
            }
        }

        Destroy(gameObject);
    }
}
