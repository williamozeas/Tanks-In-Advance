using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadEmitter : MonoBehaviour
{
    private ParticleSystem _particles;
    private ParticleSystem.MainModule _newMainModule;
    private Tank _tank;

    public Material BlueTrackMat;
    public Material PinkTrackMat;
    
    // Start is called before the first frame update
    void Start()
    {
        _particles = GetComponent<ParticleSystem>();
        _newMainModule = _particles.main;
        _tank = GetComponentInParent<Tank>();
        switch (_tank.ownerNum)
        {
            case(PlayerNum.Player1):
            {
                GetComponent<ParticleSystemRenderer>().material = BlueTrackMat;
                break;
            }
            case(PlayerNum.Player2):
            {
                GetComponent<ParticleSystemRenderer>().material = PinkTrackMat;
                break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y,
            transform.parent.position.z - 0.27f);
        _newMainModule.startRotation = Mathf.Deg2Rad * (transform.rotation.eulerAngles.y);
        // Debug.Log(newMainModule.startRotation.constant);
    }

    public void OnGhost()
    {
        _particles.Stop();
    }
}
