using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Wall
{
    public int health;
    public const int MaxHealth = 2;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        GameManager.OnRoundStart += OnRoundStart;
    }

    private void OnDisable() {
        GameManager.OnRoundStart -= OnRoundStart;
    }
    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Checks if the collided object is a bullet (hopefully)
        //TODO: Check if this works properly with bullets
        Bullet bullet;
        if(collision.gameObject.TryGetComponent<Bullet>(out bullet)){
            Destroy(bullet.gameObject);
            health--;
            // Debug.Log("Wall health: " + health);
            if(health <= 0)
            {
                // Debug.Log("Wall Destroyed");
                Die();
            }
        }
    }

    public virtual void Die(){
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null) canvas.enabled = false;
    }

    void OnRoundStart(Round round){
        //Reset health & undie
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        health = MaxHealth;
    }

}
