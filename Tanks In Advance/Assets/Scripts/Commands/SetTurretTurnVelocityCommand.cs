using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This command should be called if we decide to start or stop rotating an object
public class SetTurretTurnVelocityCommand : Command
{
    private float _ang_velocity;
    private Tank _tank;
    
    public SetTurretTurnVelocityCommand(float angVelocity, Tank tank, float timestamp)
    {
        _ang_velocity = angVelocity;
        _tank = tank;
        this.timestamp = timestamp;
    }

    public override void Execute()
    {
        // _tank.SetTurretTurnVelocity(_ang_velocity);
    }

    public override string ToString()
    {
        return _tank.name + ": Set Turret's Turn Velocity to " + _ang_velocity;
    }
}
