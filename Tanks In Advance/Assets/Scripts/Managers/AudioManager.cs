using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private FMOD.Studio.EventInstance Music;

    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Battle");
        Music.setParameterByName("Round", 0f);
        Music.start();
    }

    private void OnEnable()
    {
        GameManager.OnRoundEnd += OnRoundEnd;
        GameManager.OnRoundStart += OnRoundStart;
    }
    
    private void OnDisable()
    {
        GameManager.OnRoundEnd += OnRoundEnd;
        GameManager.OnRoundStart += OnRoundStart;
    }

    private void OnRoundStart(Round round)
    {
        Music.setParameterByName("Round", 1f + Mathf.Floor((float)round.number / GameManager.Instance.maxRounds * 3));
        Debug.Log("setting intensity to: " +
                  (1f + Mathf.Floor((float)round.number / GameManager.Instance.maxRounds * 3)));
    }

    private void OnRoundEnd()
    {
        Music.setParameterByName("Round", 0f + Mathf.Floor((float) GameManager.Instance.RoundNumber / GameManager.Instance.maxRounds));
    }

    public void Shoot(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Shoot", self);
    }

    public void BlockLay()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/BlockSet");
    }

    public void Die(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Die", self);
    }

    public void Dissipate(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Dissipate", self);
    }

    public void Destroy(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Destroy", self);
    }
    
    public void Bounce(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Bounce", self);
    } 
    public void Collide(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Collide", self);
    }

    public void Mine(GameObject self)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Game/Mine", self);
    }

    public void Select()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Select");
    }

    public void Swipe()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Swipe");
    }
}
