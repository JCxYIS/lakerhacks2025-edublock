using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Environment;

public class Unit : BE2_TargetObject
{
    public BlockController BlockController;

    [Header("Runtime")]
    public string unitId;
    public int hp = 0;
    public int fp = 0;
    public string playerId = "player1";
    public bool isPlayerControlled => BlockController.IsPlayerControlledBlocks;
    


    void Awake()
    {
        BlockController = GetComponent<BlockController>();
    }


    void Start()
    {
        name = $"Unit {unitId} controlled by {playerId}";

    }
}


public class UnitObj
{
    public string unitName;
    public GameObject prefab;
}