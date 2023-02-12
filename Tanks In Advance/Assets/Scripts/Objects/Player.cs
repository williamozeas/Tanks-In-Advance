using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Small class to store keybinds
[System.Serializable]
public class PlayerInput
{
    public KeyCode Up, Down, Left, Right, Shoot;
    public PlayerInput()
    {
        Up = KeyCode.W;
        Down = KeyCode.S;
        Left = KeyCode.A;
        Right = KeyCode.D;
        Shoot = KeyCode.Space;
    }
}

public enum PlayerNum
{
    Player1 = 0,
    Player2 = 1
}

/*
 * The Player class is separate from tanks, as different tanks will be controlled by the player at different
 * times. When the player does an input, it sends a command to the Tank class, which then stores it and executes
 * it immediately. This will enable replay.
 */
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerNum _playerNumber;
    public PlayerNum PlayerNumber => _playerNumber;
    
    [SerializeField] private Tank _currentTank;

    [SerializeField] private PlayerInput inputs;


    // Start is called before the first frame update
    void Start()
    {
        //allows GameManager to exist before scene starts
        GameManager.Instance.Players[(int)PlayerNumber] = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(GameManager.Instance.RoundTime);
        switch (GameManager.Instance.GameState)
        {
            case(GameStates.Playing): {
                //Tank movement code
                if (!_currentTank)
                {
                    break;
                }
                
                Vector2 newVelocity = Vector2.zero;
                if (Input.GetKey(inputs.Up))
                {
                    newVelocity += new Vector2(0, 1);
                }

                if (Input.GetKey(inputs.Down))
                {
                    newVelocity += new Vector2(0, -1);
                }

                if (Input.GetKey(inputs.Left))
                {
                    newVelocity += new Vector2(-1, 0);
                }

                if (Input.GetKey(inputs.Right))
                {
                    newVelocity += new Vector2(1, 0);
                }

                // Debug.Log(newVelocity);
                newVelocity = _currentTank.speed * newVelocity.normalized;
                if (newVelocity != _currentTank.Velocity)
                {
                    Command setVelocityCommand =
                        new SetVelocityCommand(newVelocity, _currentTank, GameManager.Instance.RoundTime);
                    _currentTank.AddCommand(setVelocityCommand);
                    setVelocityCommand.Execute();
                }
                break;
            }
        }
    }

    public void SetCurrentTank(Tank newTank)
    {
        _currentTank = newTank;
    }
}
