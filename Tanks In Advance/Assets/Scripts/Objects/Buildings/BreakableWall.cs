using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Wall
{
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
    }*/
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");
        //TODO: Check if collision is from a bullet
        health--;
        //Debug.Log(health);
        if(health <= 0)
        {
            Debug.Log("Destroyed");
            //Destroy it
        }
    }
}
