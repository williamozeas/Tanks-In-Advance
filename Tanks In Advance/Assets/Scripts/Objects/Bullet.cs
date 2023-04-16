using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum BulletType
{
    Pico = 0,
}

public class Bullet : MonoBehaviour
{
    protected Tank _tank;
    protected float _currentLifespan;
    public float _totalLifespan = 5f;
    public int ricochets;
    public float speed = 5;
    protected bool canHitSelf;
    public int power = 1;
    public int _maxBounces = 0;
    private Vector2 velocity;
    private TrailRenderer trailRenderer;
    public bool is_ghost;

    [SerializeField] private Gradient trailColorBlue;
    [SerializeField] private Gradient trailColorPink;

    private float _ricochetCooldown = 0.1f;
    private float _timeSinceRicochet = 0f;
    private Vector2 _previousRicochet;
    private Vector2 _previousRicochetIncident;

    protected MeshRenderer[] meshes;
    private MaterialPropertyBlock _propBlock;
    
    //called on creation
    public virtual void Init(Tank source, Vector2 angle)
    {
        _tank = source;
        velocity = angle.normalized * speed;
        _currentLifespan = _totalLifespan;
        ricochets = 0;

        meshes = GetComponentsInChildren<MeshRenderer>();
        _propBlock = new MaterialPropertyBlock();

        //Change the color of the trail based on player
        if (TryGetComponent<TrailRenderer>(out trailRenderer))
        {
            if (source.ownerNum == PlayerNum.Player1)
            {
                trailRenderer.colorGradient = trailColorBlue;
            }
            else
            {
                trailRenderer.colorGradient = trailColorPink;
            }
        }
        else
        {
            Debug.Log("no trailrenderer found!");
        }
        
        is_ghost = !source.Alive;
        if (is_ghost)
        {
            Ghostify();
        }
    }

    protected virtual void Ghostify()
    {
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        if (trailRenderer)
        {
            trailRenderer.enabled = false;
        }

        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }

        foreach (var mesh in meshes)
        {
            for (int i = 0; i < mesh.sharedMaterials.Length; i++)
            {
                Color oldC = mesh.sharedMaterials[i].GetColor("_Color");
                Color newC = new Color(oldC.r, oldC.g, oldC.b, oldC.a * 0.4f);
                MaterialMod.SetColor(newC, mesh, _propBlock, i);
            }
        }
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        this._currentLifespan = _totalLifespan;
        this.ricochets = 0;
        this.canHitSelf = false;
    }

    // Update called every frame
    protected virtual void Update()
    {
        if (this._currentLifespan < 0 || ricochets > _maxBounces)
        {
            KillSelf();
        }
        this._currentLifespan -= Time.deltaTime;
        _timeSinceRicochet += Time.deltaTime;
    }

    protected void FixedUpdate()
    {
        Vector3 actualVelocity = new Vector3(velocity.x, 0, velocity.y);
        transform.Translate(actualVelocity * Time.fixedDeltaTime);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Collider hit = collision.collider;
        Tank tank;
        Wall wall;
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Tanks") &&
            hit.transform.parent.TryGetComponent<Tank>(out tank) &&
            tank.Type != TankType.shield)
        {
            Debug.Log(tank);
            if (tank == _tank && canHitSelf) //To avoid self-destruction.
            {
                tank.TakeDamage(power);
                KillSelf(); //Destroy bullet
            } else if (tank != _tank)
            {
                tank.TakeDamage(power);
                KillSelf(); //Destroy bullet
            }
        }
        else if (hit.CompareTag("Shield_Collider"))
        {
            tank = hit.transform.parent.GetComponentInParent<Tank>();
            tank.TakeDamage(power);
            KillSelf(); //Destroy bullet
        }
        else if (hit.TryGetComponent<Wall>(out wall))
        {
            Ricochet(collision);
        }
        else
        {
            KillSelf();
        }

    }

    protected void Ricochet(Collision coll, bool quantizeNormal = true)
    {

        canHitSelf = true;
        float tolerance = 0.01f;
        
        Vector2 incident = velocity;
        Vector3 normal = coll.GetContact(0).normal;
        Vector2 normal2;

        //quantize weird normals
        if (quantizeNormal && (normal.x != 0 && normal.z != 0))
        {
            //quantize to 45 degree angles
            normal2 = new Vector2(Math.Sign(normal.x), Math.Sign(normal.z)).normalized;
        }
        else
        {
            normal2 = new Vector2(normal.x, normal.z);
            
            //quantize perpendicular collisions
            float dot = Vector2.Dot(normal2, incident);
            if (quantizeNormal && (Mathf.Abs(dot) < tolerance))
            {
                //quantize to 45 degree angles
                normal2 = new Vector2(Math.Sign(normal.x - incident.x), Math.Sign(normal.z - incident.y)).normalized;
            }
        }
        Vector2 normalComponent = Vector2.Dot(normal2, incident) * normal2;
        Debug.Log("Ricochet! " + normalComponent);

        Vector2 newVelocity = incident - 2 * normalComponent;
        
        if (_timeSinceRicochet > _ricochetCooldown)
        {
            ricochets++;
            _timeSinceRicochet = _ricochetCooldown;
            _previousRicochet = newVelocity;
            _previousRicochetIncident = incident;
        }
        else
        {
            if(Vector2.Dot(newVelocity, _previousRicochetIncident) > Vector2.Dot(_previousRicochet, _previousRicochetIncident))
            {
                
                Debug.Log("Ricochet changed to " + _previousRicochet);
                newVelocity = _previousRicochet;
            }
        }
        
        velocity = newVelocity;
    }

    protected void KillSelf(float timeToKill = 0)
    {
        if (_tank.bulletList.Contains(gameObject))
            _tank.bulletList.Remove(gameObject);

        //Destroy sound
        AudioManager.Instance.Dissipate();
        //Destroy animation
        Destroy(gameObject, timeToKill);
    }
}
