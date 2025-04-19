using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Serializer;
using MG_BlocksEngine2.Environment;

/// <summary>
/// Coding blocks
/// </summary>
public class BlockController : MonoBehaviour
{
    // public CanvasGroup _progEnvCanvasGroup;
    I_BE2_ProgrammingEnv _be2ProgrammingEnv;
    public bool IsPlayerControlledBlocks = false;

    /* -------------------------------------------------------------------------- */

    void Awake()
    {
        _be2ProgrammingEnv = GetComponentInChildren<I_BE2_ProgrammingEnv>();
    }

    void Update()
    {
        // SetEnable(BlocksManager.Instance.IsEnabled && IsPlayerControlledBlocks);
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
    }
}