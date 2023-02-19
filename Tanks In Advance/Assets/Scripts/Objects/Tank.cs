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
    public int health = 1;
    public GameObject bulletPrefab;
    public int currentHealth;

    private Vector3 _startLocation = Vector3.zero;

    protected List<Command> commandList = new List<Command>();
    public bool IsRecorded => commandList.Count > 0;
    private bool currentlyControlled = false;
    private bool alive;
    public bool Alive => alive;

    public List<GameObject> bulletList = new List<GameObject>();

    private MeshRenderer[] meshes;
    private Collider[] colliders;

    private void Awake()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    // Start will be executed when the tank spawns in
    protected override void Start()
    {
        base.Start();
        _startLocation = transform.position;
        health *= GameManager.Instance.gameParams.tankHealthMultiplier;
        speed *= GameManager.Instance.gameParams.tankSpeedMultiplier;
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
        if (IsRecorded)
        {
            rb.position = _startLocation;
        }
        velocity = Vector2.zero;
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
        if (damage <= 0)
        {
            alive = false;
            if (currentlyControlled)
            {
                Ghost();
            } else {
                Die();
            }
        }
    }

    public void Ghost()
    {
        //go transparent
        
    }

    public void Die()
    {
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
        foreach(var mesh in meshes)
        {
            mesh.enabled = true;
        }
        foreach(var collider in colliders)
        {
            collider.enabled = true;
        }
        rb.position = _startLocation;
    }

    //Requires commandList to be in order by timestamp to work properly
    public IEnumerator Replay()
    {
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
}
