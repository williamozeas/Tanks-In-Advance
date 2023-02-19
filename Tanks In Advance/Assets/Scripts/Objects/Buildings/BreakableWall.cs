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
        GameManager.OnRoundStart += OnRoundStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        //TODO: Check if collision is from a bullet
        health--;
        Debug.Log("Wall health: " + health);
        if(health <= 0)
        {
            Debug.Log("Wall Destroyed");
            Die();
        }
    }

    void Die(){
        //Disable collider & mesh renderer 
    }

    void OnRoundStart(Round round){
        //Reset health & undie
    }


}
