using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GameStatusDTO
{
    public List<int> blocks;
    public List<G_Object> obstacles;
    public List<G_Unit> units;
}

[Serializable]
public class G_Object
{
    public string type;
    public int x;
    public int y;
}

[Serializable]
public class G_Unit
{
    /// <summary>
    /// Unit ID | {ABC}{123} | ABC means type, 123 means player
    /// </summary>
    public string id;
    public int x;
    public int y;
    public int angle;
    public int hp;
    public int fp;
    public string playerId;
}

[Serializable]
public class ActionsDTO
{
    /// <summary>
    /// actions of units
    /// </summary>
    public List<ActionDTO> actions;
    public string playerId;
}

[Serializable]
public class ActionDTO
{
    /// <summary>
    /// unit to move
    /// </summary>
    public string unit;

    /// <summary>
    /// serialized blocks
    /// </summary>
    public string blocks;
}
