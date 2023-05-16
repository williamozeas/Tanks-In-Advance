using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Wall
{
    public int health;
    public int MaxHealth = 1;
    public override bool blocksLaser => false;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // float offset = Mathf.Sin(Time.time);
        // transform.position = startPos + new Vector3(offset, 0, 0);
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
            if (!bullet.is_ghost)
                TakeDamage(bullet.power);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die(){
        AudioManager.Instance.Destroy(gameObject);

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

    public virtual void Randomize()
    {
        //
    }
}
