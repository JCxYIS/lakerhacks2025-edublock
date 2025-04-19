using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BlocksManager : MonoBehaviour
{
    public static BlocksManager Instance;
    /* -------------------------------------------------------------------------- */

    public GameObject _unitButtonPrefab;
    public Transform _unitButtonsParent;

    public bool IsEnabled = false;
    public Dictionary<Unit, Button> _unitButtons = new Dictionary<Unit, Button>();

    BlockController _currentBlockController;

    /* -------------------------------------------------------------------------- */

    void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnDisable()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    /* -------------------------------------------------------------------------- */

    public void SetEnable(bool enable)
    {
        IsEnabled = enable;
    }

    /* -------------------------------------------------------------------------- */

    public void RegisterMyUnit(Unit unit)
    {
        unit.BlockController.IsPlayerControlledBlocks = true;
        var b = Instantiate(_unitButtonPrefab, _unitButtonsParent).GetComponent<Button>();
        _unitButtons[unit] = b;
        b.GetComponentInChildren<TMP_Text>().text = unit.unitId;
        b.onClick.AddListener(() =>
        {
            // onclick: enable that unit's blocks
            _currentBlockController?.SetEnable(false);
            unit.BlockController.SetEnable(true);
            print($"Unit {unit.unitId} blocks enabled, {_currentBlockController?.name} disabled");
            _currentBlockController = unit.BlockController;
        });
        b.interactable = true;
    }

    public ActionsDTO SerializeMyBlocks()
    {
        ActionsDTO actions = new ActionsDTO();
        actions.actions = new List<ActionDTO>();
        var units = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (var unit in units)
        {
            if (unit.isPlayerControlled)
            {
                actions.actions.Add(new ActionDTO()
                {
                    unit = unit.unitId,
                    blocks = unit.BlockController.SerializeBlocks()
                });
                break;
            }
        }
        return actions;
    }
}