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
        _unitButtonsParent.gameObject.SetActive(enable);

        if(!enable)
        {
            _currentBlockController?.SetEnable(false);
            _currentBlockController = null;
        }
    }

    /* -------------------------------------------------------------------------- */

    public void RegisterUnit(Unit unit, bool myUnit)
    {
        unit.BlockController.IsPlayerControlledBlocks = myUnit;
        var b = Instantiate(_unitButtonPrefab, _unitButtonsParent).GetComponent<Button>();
        b.interactable = myUnit;
        _unitButtons[unit] = b;
        b.GetComponentInChildren<TMP_Text>().text = unit.unitId;
        b.onClick.AddListener(() =>
        {
            // onclick: enable that unit's blocks
            _currentBlockController?.SetEnable(false);
            unit.BlockController.SetEnable(true);
            _currentBlockController = unit.BlockController;
        });
    }

    public ActionsDTO SerializeMyBlocks()
    {
        ActionsDTO actions = new ActionsDTO();
        actions.actions = new List<ActionDTO>();
        var units = FindObjectsByType<Unit>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var unit in units)
        {
            if (unit.isPlayerControlled)
            {
                // print($"Serialize {unit.unitId}");
                unit.BlockController.Reload();
                actions.actions.Add(new ActionDTO()
                {
                    unit = unit.unitId,
                    blocks = unit.BlockController.SerializeBlocks()
                });
            }
        }
        return actions;
    }
}