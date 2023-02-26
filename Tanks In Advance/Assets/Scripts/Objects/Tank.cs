using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.VFX;

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
    [HideInInspector] public PlayerNum ownerNum;
    public Player Owner => GameManager.Instance.Players[(int)ownerNum];
    private TankType type => TankType.basic; //can be overridden in parent class bc it's a property
    public TankType Type => type;
    [Header("Stats")]
    public float speed = 1.0f;
    public float turretTurnSpeed = 0.5f;
    public int health = 1;
    public GameObject bulletPrefab;
    public GameObject minePrefab;
    public int currentHealth;
    public float shootingCooldown = 0;
    [Header("VFX")]
    public VisualEffect vfx;
    

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
    private Coroutine replay;

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
        
        _startLocation = transform.position;
        currentlyControlled = true;
            
        //ensure command list is not empty
        Command setVelocityCommand =
            new SetVelocityCommand(Vector2.zero, this, -1);
        AddCommand(setVelocityCommand);
        setVelocityCommand.Execute();
    }

    protected void Update()
    {
        if(!currentlyControlled && GameManager.Instance.GameState == GameStates.Playing)
            Debug.Log(rb.velocity);

        //Tank can shoot when cooldown < 0.5
        shootingCooldown = Math.Max(0, shootingCooldown - Time.deltaTime);
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
        if (!Owner.IsCurrentTank(this)) //should always be true but in case we decide to spawn in tanks early
        {
            rb.position = _startLocation;
            currentlyControlled = false;
            UnDie(round);
            replay = StartCoroutine(Replay());
        }
    }
    
    public void OnRoundEnd()
    {
        roundsPassed += 1;
        rb.position = _startLocation;
        velocity = Vector2.zero;

        //Bullets from previous rounds should be removed.
        foreach (GameObject bullet in bulletList)
        {
            Destroy(bullet);
        }
        bulletList.Clear();
        SetVelocity(Vector2.zero);
    }

    public void AddCommand(Command newCommand)
    {
        commandList.Add(newCommand);
    }

    public void Shoot()
    {
        vfx.Play();
        //visuals for shooting
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
        alive = false;
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void Die()
    {
        Debug.Log("Ded?");
        alive = false;
        StopCoroutine(replay);
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
        Debug.Log(rb.useGravity);
        alive = true;
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
        shootingCooldown = 0.0f;
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
        ownerNum = newOwner;
        GameManager.Instance.Players[(int)ownerNum].SetCurrentTank(this);
    }

    public void SetAim(Vector2 newAim)
    {
        aim = newAim;
        float angle = -Vector2.SignedAngle(Vector2.up, newAim);
        turret.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void SetCollisions(Collider _coll, bool state)
    {
        foreach (Collider collider in colliders)
        {
            Physics.IgnoreCollision(_coll, collider, !state);
        }
    }

    // public void SetTurretTurnVelocity(float newVelocity)
    // {
    //     _turretTurnVelocity = newVelocity;
    //     // turretRB.rotation.
    // }
}
