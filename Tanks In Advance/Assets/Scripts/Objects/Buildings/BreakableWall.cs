using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Wall
{
    public int health;
    public const int MaxHealth = 3;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
        GameManager.OnRoundStart += OnRoundStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Bullet>()){
            health--;
            Debug.Log("Wall health: " + health);
            if(health <= 0)
            {
                Debug.Log("Wall Destroyed");
                Die();
            }
        }
    }

    void Die(){
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnRoundStart(Round round){
        //Reset health & undie
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        health = MaxHealth;
    }

}
