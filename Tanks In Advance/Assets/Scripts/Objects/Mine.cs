using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class Mine : Bullet
{
    [SerializeField] private Material blueMat;
    [SerializeField] private Material pinkMat;
    
    [SerializeField] private float explosionAnimTime = 0.5f;
    [SerializeField] private float radius = 3f;
    private bool _exploding;
    private MeshRenderer _mesh;
    private VisualEffect _vfx;
    private Color startColor;

    private MaterialPropertyBlock _propBlock;

    public Mine(Tank source)
    {
        _tank = source;
        _currentLifespan = _totalLifespan;

        _exploding = false;
    }
    
    // Start is called before the first frame update
    protected void Awake()
    {
        _currentLifespan = _totalLifespan;
        _mesh = GetComponentInChildren<MeshRenderer>();
        _vfx = GetComponentInChildren<VisualEffect>();
        _propBlock = new MaterialPropertyBlock();
    }

    public override void Init(Tank source, Vector2 angle)
    {
        if (source.Owner.PlayerNumber == PlayerNum.Player1)
        {
            _mesh.sharedMaterial = blueMat;
        }
        else
        {
            _mesh.sharedMaterial = pinkMat;
        }
        startColor = _mesh.sharedMaterial.GetColor("_EmissionColor");
        base.Init(source, angle);
    }

    protected override void Ghostify()
    {
        base.Ghostify();
        startColor = MaterialMod.ChangeHDRColorIntensity(startColor, -2f);
        // MaterialMod.SetEmissiveColor(newColor, _mesh, _propBlock);
    }
    
    protected override void GhostEffectCR()
    {
        //Do nothing
    }

    // FixedUpdate called every certain amt of time
    protected override void Update()
    {
        if (_currentLifespan < 0)
        {
            Explode();
        }
        _currentLifespan -= Time.deltaTime;
        // if (!is_ghost)
        {
            float intensityOffset = Mathf.Sin(220 * Mathf.Pow(_currentLifespan / _totalLifespan, 0.1f)) - 1.5f;
            Color newColor = MaterialMod.ChangeHDRColorIntensity(startColor, intensityOffset);
            MaterialMod.SetEmissiveColor(newColor, _mesh, _propBlock);
        }
    }

    public void Explode()
    {
        if (_exploding)
            return;

        _exploding = true;

        AudioManager.Instance.Mine();

        if (!is_ghost)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in hits)
            {
                BreakableWall bWall = hit.gameObject.GetComponent<BreakableWall>();
                if (bWall != null)
                {
                    bWall.TakeDamage(power);
                }

                // tank collision detection
                if (hit.transform.parent != null
                    && hit.gameObject.layer == LayerMask.NameToLayer("Tanks"))
                {
                    if (hit.transform.parent.TryGetComponent<Tank>(out Tank tank))
                    {
                        tank.TakeDamage(power);
                    }
                }

                // bullet collision detection
                if (hit.TryGetComponent<Bullet>(out Bullet bullet))
                {
                    // don't explode self
                    if (bullet != this)
                    {
                        if (bullet is Mine)
                            ((Mine)bullet).Explode();
                        else
                            Destroy(bullet);
                    }
                }
            }
        }

        ExplosionAnim();
    }

    private void ExplosionAnim()
    {
        _mesh.enabled = false;
        KillSelf(explosionAnimTime);
        _vfx.SetFloat("ExplosionTime", explosionAnimTime);
        _vfx.SetFloat("ExplosionSize", radius);
        bool isBlue = _tank.Owner.PlayerNumber == PlayerNum.Player1;
        _vfx.SetInt("IsBlue", isBlue ? 0 : 1); //this is reversed but don't worry about it
        _vfx.SetInt("IsGhost", is_ghost ? 1 : 0);
        _vfx.Play();
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (canHitSelf && collision.gameObject != _tank.gameObject)
            Explode();
    }

    protected void OnTriggerExit(Collider collision)
    {
        if (collision.transform.parent == _tank.transform)
            canHitSelf = true;
    }
}
