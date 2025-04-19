using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Serializer;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Core;

/// <summary>
/// Coding blocks
/// </summary>
public class BlockController : MonoBehaviour
{
    // public CanvasGroup _progEnvCanvasGroup;
    I_BE2_ProgrammingEnv _be2ProgrammingEnv;
    BE2_ExecutionManager _be2ExecutionManager;
    public bool IsPlayerControlledBlocks = false;

    /* -------------------------------------------------------------------------- */

    void Awake()
    {
        _be2ProgrammingEnv = GetComponentInChildren<I_BE2_ProgrammingEnv>();
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
        // TODO
    } 

    public string SerializeBlocks()
    {
        return BE2_BlocksSerializer.BlocksCodeToXML(_be2ProgrammingEnv);
    } 

    public void DeserializeBlocks(string xml)
    {
        BE2_BlocksSerializer.XMLToBlocksCode(xml, _be2ProgrammingEnv);
    }

    public void PlayBlocks()
    {
        // TODO
        _be2ExecutionManager.Play();
    }
}