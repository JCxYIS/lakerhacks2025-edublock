using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Environment;
using TMPro;

public class Unit : MonoBehaviour
{
    public BlockController BlockController;

    [Header("Runtime")]
    public string unitId;
    public int hp = 0;
    public int fp = 0;
    public string playerId;
    public bool isPlayerControlled => BlockController.IsPlayerControlledBlocks;
    UnitLabel _unitLabel;


    void Awake()
    {
        BlockController = GetComponent<BlockController>();
    }


    void Start()
    {
        name = $"Unit {unitId} controlled by {playerId}";
        // unitNameText.text = unitId;
        var unitLabelPrefab = Resources.Load<GameObject>("Prefabs/UnitLabel");
        _unitLabel = Instantiate(unitLabelPrefab, transform).GetComponent<UnitLabel>();
        _unitLabel.unitIdLabel.text = unitId;
    }

    void Update()
    {
        _unitLabel.hpLabel.text = $"{hp}";
        // _unitLabel.transform.position = transform.position + new Vector3(0, 1.5f, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GameController"))
        {
            var otherUnit = other.GetComponent<Unit>();
            hp -= otherUnit.fp;
        }
    }
}


public class UnitObj
{
    public string unitName;
    public GameObject prefab;
}