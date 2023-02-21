using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankType
{
    basic = 0,
}

/*
 * Tanks will recieve input from the player and store that input in a list of commands, which will then
 * be replayed on subsequent rounds.
 */
public class Tank : MovingObject
{
    [HideInInspector] public PlayerNum owner;
    private TankType type => TankType.basic; //can be overridden in parent class bc it's a property
    public TankType Type => type;
    [Header("Stats")]
    public float speed = 1.0f;
    public float turretTurnSpeed = 0.5f;
    public int health = 1;
    public GameObject bulletPrefab;
    public int currentHealth;

    private Vector3 _startLocation = Vector3.zero;
    // private float _turretTurnVelocity = 0;
    // private float turretAngle = 0;
    // public float TurretTurnVelocity => _turretTurnVelocity;
    [HideInInspector] private Vector2 aim = Vector2.up;
    public Vector2 Aim => aim;
    
    public Rigidbody turretRB;
    public List<Renderer> renderers;

    private int roundsPassed = 0;
    protected List<Command> commandList = new List<Command>();
    public bool IsRecorded => commandList.Count > 0;
    private bool currentlyControlled = false;
    private bool alive;
    public bool Alive => alive;

    public List<GameObject> bulletList = new List<GameObject>();

    private MeshRenderer[] meshes;
    private Collider[] colliders;
    private Turret turret;

    private void Awake()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
        turret = GetComponentInChildren<Turret>();
    }

    // Start will be executed when the tank spawns in
    protected override void Start()
    {
        base.Start();
        _startLocation = transform.position;
        health *= GameManager.Instance.gameParams.tankHealthMultiplier;
        speed *= GameManager.Instance.gameParams.tankSpeedMultiplier;
    }

    protected void Update()
    {
        base.FixedUpdate();
        // turretAngle = turretTurnSpeed * _turretTurnVelocity;
        
    }
    
    //Subscribe to events
    private void OnEnable()
    {
        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnRoundEnd += OnRoundEnd;
    }

    private void OnDisable()
    {
        GameManager.OnRoundStart -= OnRoundStart;
        GameManager.OnRoundEnd -= OnRoundEnd;
    }

    public void OnRoundStart(Round round)
    {
        if (IsRecorded)
        {
            currentlyControlled = false;
            UnDie(round);
            StartCoroutine(Replay());
        }
        else
        {
            _startLocation = transform.position;
            currentlyControlled = true;
            
            //ensure command list is not empty
            Command setVelocityCommand =
                new SetVelocityCommand(Vector3.zero, this, -1);
            AddCommand(setVelocityCommand);
            setVelocityCommand.Execute();
        }
    }
    
    public void OnRoundEnd()
    {
        roundsPassed += 1;
        if (IsRecorded)
        {
            rb.position = _startLocation;
        }
        velocity = Vector2.zero;

        //Bullets from previous rounds should be removed.
        bulletList.Clear();
    }

    public void AddCommand(Command newCommand)
    {
        commandList.Add(newCommand);
    }

    public void Shoot()
    {
        //visuals for shooting
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            alive = false;
            if (currentlyControlled)
            {
                Ghost();
            }
            else {
                Die();
            }
        }
    }

    public void Ghost()
    {
        Debug.Log("Spectating!");
    }

    public void Die()
    {
        Debug.Log("Ded?");
        foreach(var mesh in meshes)
        {
            mesh.enabled = false;
        }
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void UnDie(Round round)
    {
        Debug.Log("Arise! My son.");
        foreach(var mesh in meshes)
        {
            mesh.enabled = true;
        }
        foreach(var collider in colliders)
        {
            collider.enabled = true;
        }
        rb.position = _startLocation;
        currentHealth = health;
    }

    //Requires commandList to be in order by timestamp to work properly
    public IEnumerator Replay()
    {
        // make the tank more transparent based on rounds passed
        foreach (Renderer r in renderers)
        {
            Color oldC = r.material.color;
            Color newC = new Color(oldC.r, oldC.g, oldC.b, oldC.a * 0.8f);
            r.material.color = newC;
        }
        
        var enumerator = commandList.GetEnumerator();
        // for (int i = 0; i < commandList.Count; i++)
        while(enumerator.MoveNext())
        {
            Command nextCommand = enumerator.Current;
            //wait for time to be at correct timestamp
            while (GameManager.Instance.RoundTime < nextCommand.Timestamp)
            {
                yield return new WaitForFixedUpdate();
            }
            nextCommand.Execute();
            // Debug.Log("Set Velocity to " + nextCommand.ToString());
        }
    }

    //aesthetic changes based on team
    public void AssignToTeam(PlayerNum newOwner)
    {
        owner = newOwner;
    }

    public void SetAim(Vector2 newAim)
    {
        aim = newAim;
        float angle = -Vector2.SignedAngle(Vector2.up, newAim);
        turret.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    
    // public void SetTurretTurnVelocity(float newVelocity)
    // {
    //     _turretTurnVelocity = newVelocity;
    //     // turretRB.rotation.
    // }
}
