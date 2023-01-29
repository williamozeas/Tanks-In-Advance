using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    EndGame
}

//Singleton class means it can be ref'd with GameManger.Instance anywhere
public class GameManager : Singleton<GameManager> 
{
    private GameStates _gameState = GameStates.Playing;
    public GameStates GameState
    {
        //get/set means that gamestate can't be changed without calling SetGameState();
        get { return _gameState; }
        set { SetGameState(value); }
    }
    
    private float _roundTime = 0.0f;
    public float RoundTime => _roundTime; //shorthand to make round time unmodifiable from outside class
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState == GameStates.Playing)
        {
            _roundTime += Time.deltaTime;
        }
    }

    public void SetGameState(GameStates newGameState)
    {
        _gameState = newGameState;
    }
}
