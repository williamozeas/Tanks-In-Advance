using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Shoot");
    }

    public void BlockLay()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Shoot");
    }

    public void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Die");
    }
}
