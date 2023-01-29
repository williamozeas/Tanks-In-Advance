using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    MainMenu,
    Playing,
    BetweenRounds,
    Paused,
    EndGame
}

//Singleton class means it can be ref'd with GameManger.Instance anywhere
public class GameManager : Singleton<GameManager> 
{
    private GameStates _gameState = GameStates.BetweenRounds;
    public GameStates GameState
    {
        //get/set means that gamestate can't be changed without calling SetGameState();
        get { return _gameState; }
        set { SetGameState(value); }
    }

    [HideInInspector]
    public Player[] Players = new Player[2];
    // public List<Player> Players = new List<Player>(2);

    //Events
    //These events will be called when the game state is changed. When an event is called, all subscribed
    //functions fire. To subscribe a function to an event, write "EventName += FnName" inside an OnEnable 
    //function, and "EventName -= FnName" inside an OnDisable() function.
    public static event Action<Round> OnRoundStart;
    public static event Action OnRoundEnd;
    public static event Action OnGameEnd;
    public static event Action OnMainMenu;

    
    private float _roundTime = 0.0f;
    public float RoundTime => _roundTime; //shorthand to make round time unmodifiable from outside class
    
    private int _roundNumber = 1;
    public int RoundNumber => _roundNumber; //shorthand to make round # unmodifiable from outside class

    public override void Awake()
    {
        base.Awake();
        Players = new Player[2];
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameState == GameStates.Playing)
            {
                GameState = GameStates.BetweenRounds;
            }
            else
            {
                GameState = GameStates.Playing;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameState == GameStates.Playing)
        {
            _roundTime += Time.fixedDeltaTime;
        }
    }

    public void SetGameState(GameStates newGameState)
    {
        switch (newGameState)
        {
            case(GameStates.Playing):
            {
                if (OnRoundStart != null) OnRoundStart(new Round());
                _roundTime = 0;
                break;
            }
            case(GameStates.BetweenRounds):
            {
                if (OnRoundEnd != null) OnRoundEnd();
                break;
            }
        }
        _gameState = newGameState;
    }
}
