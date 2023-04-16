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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/BlockSet");
    }

    public void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Die");
    }

    public void startBattleMusic()
    {
        FMODUnity.RuntimeManager.CreateInstance("event:/Music/Battle");
    }

    public void Destroy()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Destroy");
    }
    
    public void Bounce()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Bounce");
    } 
    public void Collide()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Collide");
    }

    public void Mine()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Mine");
    }

    public void Select()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Select");
    }

    public void Swipe()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Swipe");
    }

    //Engine
    public void moveSound(float speed)
    {
        
    }
    
}
