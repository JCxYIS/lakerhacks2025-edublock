using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Environment;
using TMPro;

public class Unit : MonoBehaviour
{
    public BlockController BlockController;
    public TMP_Text unitNameText;

    [Header("Runtime")]
    public string unitId;
    public int hp = 0;
    public int fp = 0;
    public string playerId;
    public bool isPlayerControlled => BlockController.IsPlayerControlledBlocks;
    


    void Awake()
    {
        BlockController = GetComponent<BlockController>();
    }


    void Start()
    {
        name = $"Unit {unitId} controlled by {playerId}";
        // unitNameText.text = unitId;
    }
}


public class UnitObj
{
    public string unitName;
    public GameObject prefab;
}