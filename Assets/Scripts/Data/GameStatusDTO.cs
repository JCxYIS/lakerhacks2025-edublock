using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GameStatusDTO
{
    public List<int> blocks;
    public List<G_Object> objects;
    public List<G_Unit> units;
}

[Serializable]
public class G_Object
{
    public string name;
    public int x;
    public int y;
}

[Serializable]
public class G_Unit
{
    public int unitId;
    public int x;
    public int y;
    public int hp;
    public int fp;
    public int player;
}
