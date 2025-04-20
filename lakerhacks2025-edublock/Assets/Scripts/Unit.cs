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
    public bool isDead = false;
    UnitLabel _unitLabel;

    Vector2 GridPosition => XZ(transform.position-GameController.Instance._worldContainer.transform.position);
    Vector2 XZ(Vector3 v) => new Vector2(v.x, v.z);

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

        if(isDead)
        {
            transform.position = new Vector3(0, -10, 0);
            return;
        }
        
        var pos = GridPosition;
        if(pos.x < 0 || pos.y < 0 || pos.x >= 5 || pos.y >= 5)
        {
            Dead();
            // enabled = false;
        }
    }

    void Dead()
    {
        if(isDead) return;
        isDead = true;

        hp = 0;

        // play explosion fx at position
        PlayFX("Explosion", .5f);

        print($"Unit {name} is Dead");
        transform.position = new Vector3(0, -10, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GameController"))
        {
            // other unit
            var otherUnit = other.GetComponent<Unit>();
            hp -= otherUnit.hp;
            PlayFX("Explosion", .25f);
        }
        else if(other.CompareTag("Finish"))
        {
            // bullet
            hp -= 1;
            PlayFX("Explosion", 0.15f);
        }
        else if(other.CompareTag("Heal"))
        {
            // heal
            hp += 3;
            // PlayFX("Heal", .15f);
        }
        else if(other.CompareTag("Wall"))
        {
            // obstacle
            hp -= 2;
            PlayFX("Explosion", 0.15f);
        }

        if(hp <= 0)
        {
            Dead();
        }
    }

    void Heal()
    {

    }


    void PlayFX(string fxName, float scale = .3f)
    {
        var fx = Instantiate(Resources.Load<GameObject>($"FX/{fxName}"), transform.position, Quaternion.identity);
        fx.transform.localScale = new Vector3(scale, scale, scale);
        Destroy(fx, 5);
    }
}


public class UnitObj
{
    public string unitName;
    public GameObject prefab;
}