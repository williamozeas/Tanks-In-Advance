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
    mine = 1,
    shield = 2,
    laser = 3
}

/*
 * Tanks will recieve input from the player and store that input in a list of commands, which will then
 * be replayed on subsequent rounds.
 */
public class Tank : MovingObject
{
    public string tankName;

    [HideInInspector] public PlayerNum ownerNum;
    public Player Owner => GameManager.Instance.Players[(int)ownerNum];
    protected virtual TankType type => TankType.basic; //can be overridden in parent class bc it's a property
    public TankType Type => type;
    [Header("Stats")]
    public float speed = 1.0f;
    public float turretTurnSpeed = 0.5f;
    public int health = 1;
    public GameObject bulletPrefab;
    public int currentHealth;
    public float cooldown = 1f;
    [HideInInspector]
    public float shootingCooldown = 0;
    [FormerlySerializedAs("vfx")] [Header("VFX")]
    public VisualEffect ShootVfx;

    public VisualEffect DeathVfx;

    [HideInInspector]
    public bool disableMovement = false;

    // public Material ghostMat;
    

    private Vector3 _startLocation = Vector3.zero;
    protected Vector2 aim = Vector2.up;
    public Vector2 Aim => aim;

    private int roundsPassed = 0;
    protected List<Command> commandList = new List<Command>();
    public bool IsRecorded => commandList.Count > 0;
    private bool currentlyControlled = false;
    private bool alive;
    public bool Alive => alive;

    [HideInInspector]
    public List<GameObject> bulletList = new List<GameObject>();

    private MeshRenderer[] meshes;
    // private List<Material> origMat = new List<Material>();
    private Collider[] colliders;
    private Turret turret;
    private Coroutine replay;
    private TreadEmitter _treadEmitter;

    protected virtual void Awake()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
        
        // foreach (var mesh in meshes)
        // {
        //     foreach (var material in mesh.materials)
        //     {
        //         origMat.Add(material);
        //     }
        // }
        
        colliders = GetComponentsInChildren<Collider>();
        turret = GetComponentInChildren<Turret>();
        _treadEmitter = GetComponentInChildren<TreadEmitter>();
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
        alive = true;
        currentHealth = health;
            
        //ensure command list is not empty
        Command setVelocityCommand =
            new SetVelocityCommand(Vector2.zero, this, -1);
        AddCommand(setVelocityCommand);
        setVelocityCommand.Execute();
    }

    protected virtual void Update()
    {
        // if(!currentlyControlled && GameManager.Instance.GameState == GameStates.Playing)
        //     Debug.Log(rb.velocity);
        //
        // Debug.Log(Input.GetAxis("Horizontal"));
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

    public virtual void OnRoundStart(Round round)
    {
        if (!Owner.IsCurrentTank(this)) //should always be true but in case we decide to spawn in tanks early
        {
            rb.position = _startLocation;
            currentlyControlled = false;
            UnDie(round);
            replay = StartCoroutine(Replay());
        }
    }
    
    public virtual void OnRoundEnd()
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

    public virtual void AddCommand(Command newCommand)
    {
        commandList.Add(newCommand);
    }

    public virtual void Shoot(ShootCommand shootCommand)
    {
        //Shield Tank can't shoot
        if(type == TankType.shield) return;
        //The tank does nothing if shooting has not yet cooled down.
        if (shootingCooldown > 0.5f) return;

        AudioManager.Instance.Shoot();
        //visuals for shooting
        ShootVfx.Play();

        //Tank must wait to shoot again.
        if (shootingCooldown < 0.1f)
        {
            shootingCooldown += 0.6f; //Rapid barrage after waiting
        }
        else
        {
            shootingCooldown += 0.8f; //Space out following shots.
        }
        //_tank.shootingCooldown += 0.2f * _tank.rb.velocity.magnitude; //Potential


        GameObject bullet = Instantiate(
            bulletPrefab,
            rb.position + new Vector3(shootCommand._angle.x, 0, shootCommand._angle.y) * 1f,
            Quaternion.Euler(shootCommand._angle.x, 0, shootCommand._angle.y)
        );

        Bullet bulletBullet = bullet.GetComponent<Bullet>();
        bulletBullet.Init(this, shootCommand._angle);

        bulletList.Add(bullet);
    }

    public void TakeDamage(int damage)
    {
        // Debug.Log("TakeDamage called on: " + type);
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

    private void ChangeLayer(Transform tran, int layer)
    {
        if (tran == null)
        {
            return;
        }
        
        foreach (Transform child in tran)
        {
            if (child == null)
            {
                continue;
            }
            
            child.gameObject.layer = layer;
            ChangeLayer(child, layer);
        }
    }

    public virtual void Ghost()
    {
        Debug.Log("Spectating!");
        DeathVfx.Stop();
        DeathVfx.SetInt("IsBlue", (int)ownerNum);
        DeathVfx.Play();
        _treadEmitter.StopParticles();
        alive = false;
        ChangeLayer(transform, LayerMask.NameToLayer("Ghost"));
        foreach (var mesh in meshes)
        {
            for (int i = 0; i < mesh.materials.Length; i++)
            {
                Color oldC = mesh.materials[i].color;
                Color newC = new Color(oldC.r, oldC.g, oldC.b, oldC.a * 0.12f);
                mesh.materials[i].color = newC;
            }
        }
    }

    public virtual void Die()
    {
        alive = false;
        
        DeathVfx.SetInt("IsBlue", (int)ownerNum);
        DeathVfx.Play();
        _treadEmitter.StopParticles();
        if (replay != null)
        {
            StopCoroutine(replay);
        }

        ChangeLayer(transform, LayerMask.NameToLayer("Dead"));
        foreach(var mesh in meshes)
        {
            mesh.enabled = false;
        }
        // foreach(var collider in colliders)
        // {
        //     collider.enabled = false;
        // }
    }

    public virtual void UnDie(Round round)
    {
        Debug.Log(rb.useGravity);
        alive = true;
        foreach (var mesh in meshes)
        {
            mesh.enabled = true;
            for (int i = 0; i < mesh.materials.Length; i++)
            {
                Color oldC = mesh.materials[i].color;
                Color newC = new Color(oldC.r, oldC.g, oldC.b, 1);
                mesh.materials[i].color = newC;
            }
        }
        foreach(var collider in colliders)
        {
            collider.enabled = true;
        }
        rb.position = _startLocation;
        currentHealth = health;
        shootingCooldown = 0.0f;
        _treadEmitter.StartParticles();
        ChangeLayer(transform, LayerMask.NameToLayer("Tanks"));
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
        ownerNum = newOwner;
        GameManager.Instance.Players[(int)ownerNum].SetCurrentTank(this);
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
