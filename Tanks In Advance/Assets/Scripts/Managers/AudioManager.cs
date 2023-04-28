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
        GameManager.OnGameEnd += OnGameEnd;
    }
    
    private void OnDisable()
    {
        GameManager.OnRoundEnd += OnRoundEnd;
        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnGameEnd -= OnGameEnd;
    }

    private void OnRoundStart(Round round)
    {
        Music.setParameterByName("Round", 1f + Mathf.Floor((float)round.number / GameManager.Instance.maxRounds * 3));
    }

    private void OnRoundEnd()
    {
        Music.setParameterByName("Round", 0f + Mathf.Floor((float) GameManager.Instance.RoundNumber / GameManager.Instance.maxRounds));
    }

    private void OnGameEnd(PlayerNum winner)
    {
        Music.setParameterByName("Round", 0f);
    }

    public void Shoot()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Shoot");
    }

    public void BlockLay()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/BlockSet");
    }

    public void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Die");
    }

    public void Dissipate()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Game/Dissipate");
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
}
