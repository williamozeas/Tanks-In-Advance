using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A Command represents an action by the player. These will be stored for replay in a list.
 */
public abstract class Command
{
    protected float timestamp = -1;
    public abstract void Execute();
}
