using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTank : Tank
{
    protected override TankType type => TankType.shield;
    //TODO: Make the collider kill this tank
}
