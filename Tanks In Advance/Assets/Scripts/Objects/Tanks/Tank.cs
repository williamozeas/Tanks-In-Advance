using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
    public RuntimeAnimatorController tankUIAnimation;

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
    public float cooldown = 0.8f;
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
    private FMOD.Studio.EventInstance engine;
    private float engineVolume;

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
        engineVolume = 1;
        engine = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Game/Move");
        engine.start();
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engine, transform);
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
        
        //Set aim to be downwards
        Command setAimCommand = new SetAimCommand(new Vector2(0, -1), this, -1);
        AddCommand(setAimCommand);
        setAimCommand.Execute();
    }

    protected virtual void Update()
    {
        // if(!currentlyControlled && GameManager.Instance.GameState == GameStates.Playing)
        //     Debug.Log(rb.velocity);
        //
        // Debug.Log(Input.GetAxis("Horizontal"));
        //Tank can shoot when cooldown < 0.5
        shootingCooldown = Math.Max(0, shootingCooldown - Time.deltaTime);
        //engine
        engine.setParameterByName("Speed", (rb.velocity.magnitude / speed) * engineVolume);
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
        if (alive || type != TankType.basic)
        {
            ShootVfx.Play();
        }

        //Tank must wait to shoot again.
        if (shootingCooldown < 0.1f)
        {
            shootingCooldown += 0.8f * cooldown; //Rapid barrage after waiting
        }
        else
        {
            shootingCooldown += cooldown; //Space out following shots.
        }
        //_tank.shootingCooldown += 0.2f * _tank.rb.velocity.magnitude; //Potential

        
        //prevent shooting inside walls
        RaycastHit hit;
        if (Physics.Raycast(rb.position, new Vector3(shootCommand._angle.x, 0, shootCommand._angle.y), out hit, 1.5f, LayerMask.GetMask("Walls"),
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Destroyed bullet in wall");
            AudioManager.Instance.Dissipate();
            // bulletBullet.Ricochet(hit.normal);
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            rb.position + new Vector3(shootCommand._angle.x, 0.5f, shootCommand._angle.y) * 1f,
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
            if (child == null || child.gameObject.layer == LayerMask.NameToLayer("Default"))
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
        AudioManager.Instance.Die();
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
        
        TankManager.Instance.OnTankDeath();
    }

    public virtual void Die()
    {
        alive = false;
        
        DeathVfx.SetInt("IsBlue", (int)ownerNum);
        DeathVfx.Play();
        AudioManager.Instance.Die();
        engine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
        
        TankManager.Instance.OnTankDeath();
        // foreach(var collider in colliders)
        // {
        //     collider.enabled = false;
        // }
    }

    public virtual void UnDie(Round round)
    {
        alive = true;
        engine.start();
        engineVolume = .5F;
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
        if (turret)
        {
            turret.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }


    // public void SetTurretTurnVelocity(float newVelocity)
    // {
    //     _turretTurnVelocity = newVelocity;
    //     // turretRB.rotation.
    // }
}
