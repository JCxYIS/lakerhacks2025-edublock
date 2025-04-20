using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Serializer;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Block;
using System.Linq;
using System;

/// <summary>
/// Coding blocks
/// </summary>
public class BlockController : MonoBehaviour
{
    // public CanvasGroup _progEnvCanvasGroup;
    public BE2_ProgrammingEnv _be2ProgrammingEnv;
    BE2_ExecutionManager _be2ExecutionManager;
    public bool IsPlayerControlledBlocks = false;

    /* -------------------------------------------------------------------------- */

    void Awake()
    {
        // _be2ProgrammingEnv = GetComponentInChildren<BE2_ProgrammingEnv>();
        _be2ExecutionManager = FindAnyObjectByType<BE2_ExecutionManager>();
    }


    /* -------------------------------------------------------------------------- */

    public void SetEnable(bool enable)
    {
        _be2ProgrammingEnv.Visible = enable;
    }

    /* -------------------------------------------------------------------------- */

    public void ResetBlocks()
    {
        _be2ProgrammingEnv.ClearBlocks();
    } 

    public string SerializeBlocks()
    {
        return BE2_BlocksSerializer.BlocksCodeToXML(_be2ProgrammingEnv);
    } 

    public void DeserializeBlocks(string xml)
    {
        BE2_BlocksSerializer.XMLToBlocksCode(xml, _be2ProgrammingEnv);
    }

    public void Reload()
    {
        _be2ProgrammingEnv.UpdateBlocksList();
        _be2ExecutionManager.UpdateProgrammingEnvsList();
        _be2ExecutionManager.UpdateBlocksStackList();
    }

    [Obsolete]
    public void PlayBlocks()
    {
        // _be2ProgrammingEnv.UpdateBlocksList();

        // List<I_BE2_Block> orderedBlocks = new List<I_BE2_Block>();
        // orderedBlocks.AddRange(_be2ProgrammingEnv.BlocksList.OrderBy(BE2_BlocksSerializer.OrderOnType));
        // string s = $"{name} PlayBlocks BLK={orderedBlocks.Count} | ";
        // foreach (var b in orderedBlocks)
        // {
        //     s += $"{b} | ";
        // }
        // print(s);

        // _be2ExecutionManager.UpdateProgrammingEnvsList();
        // _be2ExecutionManager.UpdateBlocksStackList();
        // _be2ExecutionManager.Play();

    }

    
}