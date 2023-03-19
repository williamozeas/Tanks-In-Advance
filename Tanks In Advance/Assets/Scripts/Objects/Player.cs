using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Small class to store keybinds
[System.Serializable]
public class PlayerInput
{
    public KeyCode Up, Down, Left, Right, Fire, AltFire, AimUp, AimDown, AimLeft, AimRight;
    public PlayerInput()
    {
        Up = KeyCode.W;
        Down = KeyCode.S;
        Left = KeyCode.A;
        Right = KeyCode.D;
        Fire = KeyCode.LeftShift;
        AltFire = KeyCode.Z;
        AimUp = KeyCode.I;
        AimDown = KeyCode.K;
        AimLeft = KeyCode.J;
        AimRight = KeyCode.L;
    }
}

public enum PlayerNum
{
    Player1 = 0,
    Player2 = 1,
    Neither = 2
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

    private Rigidbody m_Rb;


    // Start is called before the first frame update
    void Start()
    {
        //allows GameManager to exist before scene starts
        GameManager.Instance.Players[(int)PlayerNumber] = this;
        m_Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(GameManager.Instance.RoundTime);
        switch (GameManager.Instance.GameState)
        {
            case(GameStates.MainMenu):
            case(GameStates.Playing): {
                //Tank movement code
                if (!_currentTank)
                {
                    break;
                }

                float horizontalInput = 0;
                float verticalInput = 0;
                // Vector2 newVelocity = Vector2.zero;
                if (PlayerNumber == PlayerNum.Player1)
                {
                    horizontalInput = Input.GetAxis("P1_Move_H");
                    verticalInput = Input.GetAxis("P1_Move_V");
                } else if (PlayerNumber == PlayerNum.Player2)
                {
                    horizontalInput = Input.GetAxis("P2_Move_H");
                    verticalInput = Input.GetAxis("P2_Move_V");
                }

                Vector2 newVelocity = new Vector2(horizontalInput, verticalInput);
                
                // if (Input.GetKey(inputs.Up))
                // {
                //     newVelocity += new Vector2(0, 1);
                // }
                //
                // if (Input.GetKey(inputs.Down))
                // {
                //     newVelocity += new Vector2(0, -1);
                // }
                //
                // if (Input.GetKey(inputs.Left))
                // {
                //     newVelocity += new Vector2(-1, 0);
                // }
                //
                // if (Input.GetKey(inputs.Right))
                // {
                //     newVelocity += new Vector2(1, 0);
                // }
                
                float rHorizontalInput = 0;
                float rVerticalInput = 0;
                // Vector2 newVelocity = Vector2.zero;
                if (PlayerNumber == PlayerNum.Player1)
                {
                    rHorizontalInput = Input.GetAxis("P1_Aim_H");
                    rVerticalInput = Input.GetAxis("P1_Aim_V");
                } else if (PlayerNumber == PlayerNum.Player2)
                {
                    rHorizontalInput = Input.GetAxis("P2_Aim_H");
                    rVerticalInput = Input.GetAxis("P2_Aim_V");
                }

                Vector2 newAim = new Vector2(rHorizontalInput, rVerticalInput);
                Debug.Log(newAim);
                
                //TEMP FOR CONTROLLER
                // if (Input.GetKey(inputs.AimUp))
                // {
                //     newAim += new Vector2(0, 1);
                // }
                //
                // if (Input.GetKey(inputs.AimDown))
                // {
                //     newAim += new Vector2(0, -1);
                // }
                //
                // if (Input.GetKey(inputs.AimLeft))
                // {
                //     newAim += new Vector2(-1, 0);
                // }
                //
                // if (Input.GetKey(inputs.AimRight))
                // {
                //     newAim += new Vector2(1, 0);
                // }
                
                Vector2 newAimNorm = newAim.normalized;
                if (newAimNorm != _currentTank.Aim) //controller dead zone
                {
                    Command setAimCommand =
                        new SetAimCommand(newAimNorm, _currentTank, GameManager.Instance.RoundTime);
                    _currentTank.AddCommand(setAimCommand);
                    setAimCommand.Execute();
                }

                if (Input.GetButtonDown("P1_Fire")) Debug.Log("PIZZA WOOO");

                // fire button pressed (shooting for now)
                if ((PlayerNumber == PlayerNum.Player1 && Input.GetButtonDown("P1_Fire")) ||
                     PlayerNumber == PlayerNum.Player2 && Input.GetButtonDown("P2_Fire"))
                {
                    // Vector2 angle = _currentTank.Velocity.normalized;
                    Vector2 angle = _currentTank.Aim;
                    Command shootCommand = 
                            new ShootCommand(angle, _currentTank, GameManager.Instance.RoundTime);
                    _currentTank.AddCommand(shootCommand);
                    shootCommand.Execute();
                }

                // alt fire button pressed
                if (Input.GetKeyDown(inputs.AltFire))
                {
                    /*
                    Vector2 angle = _currentTank.Velocity.normalized;
                    Command mineCommand =
                            new MineCommand(_currentTank, GameManager.Instance.RoundTime);
                    _currentTank.AddCommand(mineCommand);
                    mineCommand.Execute();
                    */
                }

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

    public bool IsCurrentTank(Tank compare)
    {
        return compare == _currentTank;
    }
}
