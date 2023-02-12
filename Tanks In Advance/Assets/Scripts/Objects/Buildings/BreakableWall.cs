using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Wall
{
    public int health = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
    }*/
    
    void OnColliderEnter(Collision collision)
    {
        Debug.Log("collided");
    }
}
