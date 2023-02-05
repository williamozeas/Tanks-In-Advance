using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This command represents a change in speed or direction via an input by the player.
 * This does not necessarily mean it will be the actual velocity, because we may use
 * smoothing functions etc. in the future.
 */
public class SetVelocityCommand : Command
{
    private Vector2 _velocity;
    private Tank _tank;
    
    public SetVelocityCommand(Vector2 velocity, Tank tank, float timestamp)
    {
        _velocity = velocity;
        _tank = tank;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        _tank.SetVelocity(_velocity);
    }

    public override string ToString()
    {
        return _tank.name + ": Set Velocity to " + _velocity;
    }
}

