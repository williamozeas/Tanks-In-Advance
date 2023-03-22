using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    private GameStates _gameState = GameStates.MainMenu;
    public GameStates GameState
    {
        //get/set means that gamestate can't be changed without calling SetGameState();
        get { return _gameState; }
        set { SetGameState(value); }
    }

    [Header("Parameters")] 
    public int maxRounds = 5;

    [Header("References")] [SerializeField]
    private TankList _tankList;
    public TankList TankList => _tankList;
    [SerializeField]
    private MapList _mapList;
    public MapList MapList => _mapList;
    public GameParams gameParams;
    
    
    [HideInInspector]
    public Player[] Players = new Player[2];

    [HideInInspector] public MapSpawner mapSpawner;

    //Events
    //These events will be called when the game state is changed. When an event is called, all subscribed
    //functions fire. To subscribe a function to an event, write "EventName += FnName" inside an OnEnable 
    //function, and "EventName -= FnName" inside an OnDisable() function.
    public static event Action<Round> OnRoundStart;
    public static event Action OnRoundEnd;
    public static event Action<PlayerNum> OnGameEnd;
    public static event Action OnMainMenu;

    private float _roundTime = 0.0f;
    public float RoundTime => _roundTime; //shorthand to make round time unmodifiable from outside class
    
    private int _roundNumber = 0;
    public int RoundNumber => _roundNumber; //shorthand to make round # unmodifiable from outside class

    public override void Awake()
    {
        base.Awake();
        Players = new Player[2];
        _tankList.Init();
        _mapList.Init();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (GameState == GameStates.Playing)
        {
            _roundTime += Time.deltaTime;
            if(_roundTime >= gameParams.time){
                Debug.Log("End of Round");
                GameState = GameStates.BetweenRounds;
            }
        } else if(Input.GetKeyDown(KeyCode.R)){ //FOR DEBUG
            if(GameState == GameStates.MainMenu)
                GameState = GameStates.BetweenRounds;
            else
                GameState = GameStates.Playing;
        }
    }

    public void SetGameState(GameStates newGameState)
    {
        Debug.Log("new game state: " + newGameState);
        switch (newGameState)
        {
            case(GameStates.Playing):
            {
                _roundTime = 0;
                if (OnRoundStart != null){
                    OnRoundStart(new Round() { number = _roundNumber });
                }
                break;
            }
            case(GameStates.BetweenRounds):
            {
                _roundNumber++;
                if (_roundNumber > maxRounds)
                {
                    SetGameState(GameStates.EndGame);
                }
                if (OnRoundEnd != null) OnRoundEnd();
                break;
            }
            case (GameStates.EndGame):
            {
                OnGameEnd?.Invoke(mapSpawner.CurrentMap.GetWinner());
                break;
            }
        }
        _gameState = newGameState;
    }
}
