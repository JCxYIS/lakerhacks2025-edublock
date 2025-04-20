using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Core;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public string playerId = "player1";
    public List<GameObject> _unitObjs;
    public List<BE2_ProgrammingEnv> _unitEnvs;
    public Transform _worldContainer;
    public PlayAnimationButton _playButtonDummy;

    /* -------------------------------------------------------------------------- */
    [Header("Runtime")]
    public int _countDownTimer = 0;


    [SerializeField]
    public List<GameObject> _objects = new List<GameObject>();


    /* -------------------------------------------------------------------------- */

    public enum GamePhase { Init, ArrangeBlocks, WaitForOthers, Animation, GG }
    public GamePhase _phase = GamePhase.Init;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Instance = this;   
        _playButtonDummy.gameObject.SetActive(false);
        
        var menuData = FindAnyObjectByType<MenuDataPasser>();
        if(menuData)
        {
            playerId = menuData.playerName;
            Destroy(menuData.gameObject);
        }
    }

    void Start()
    {
        WebApiHelper.ROOT_URL = "https://0324-129-3-208-193.ngrok-free.app/";
        WebApiHelper.USE_API_RESPONSE_MODEL = true;
        WebApiHelper.CallApi<GameStatusDTO>("GET", $"game-state?playerId={playerId}", (succ, data)=>{
            if(!succ || data == null)
            {
                Debug.LogError("Failed to get game status");
                return;
            }

            InitGame(data);
        });
        _phase = GamePhase.Init;
    }

    void Update()
    {
        switch(_phase)
        {
            case GamePhase.ArrangeBlocks:
                BlocksManager.Instance.SetEnable(true);
                break;
            default:
                BlocksManager.Instance.SetEnable(false);
                break;
        }
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Initialize the game with the given game status
    /// </summary>
    /// <param name="gameStatus"></param>
    void InitGame(GameStatusDTO gameStatus)
    {
        // TODO Initialize blocks
        foreach (var block in gameStatus.blocks)
        {
            
        }

        // TODO Initialize objects
        foreach (var obj in gameStatus.obstacles)
        {
            // GameObject objPrefab = Resources.Load<GameObject>("Obstacles/"+obj.name);
            // if (objPrefab != null)
            // {
            //     var u = Instantiate(objPrefab, new Vector3(obj.x, 0, obj.y), Quaternion.identity);
                
            // }
        }

        // init 
        for(int i = 0; i < 4; i++)  // FIXME max 2 user x 2 units
        {
            var u = _unitObjs[i].AddComponent<Unit>();
            
            var unitData = gameStatus.units[i];
            u.transform.position = new Vector3( unitData.x, 0, unitData.y) + _worldContainer.position;
            u.transform.localRotation = Quaternion.Euler(0, unitData.angle, 0);
            u.playerId = unitData.playerId;
            u.unitId = unitData.id;
            u.hp = unitData.hp;
            u.fp = unitData.fp;
            
            u.BlockController = _unitObjs[i].AddComponent<BlockController>();
            u.BlockController._be2ProgrammingEnv = _unitEnvs[i];
            BlocksManager.Instance.RegisterUnit(u, u.playerId == playerId);
        }

        _phase = GamePhase.ArrangeBlocks;
        _countDownTimer = 30;
    }

    /* -------------------------------------------------------------------------- */

    public void ConfirmBlock()
    {
        if(_phase == GamePhase.WaitForOthers)
        {
            // Debug.Log("Already in waiting phase");
            return;
        }

        _phase = GamePhase.WaitForOthers;

        // test 
        // StartCoroutine(WaitForOthersAsync());
        // return;
        
        // call api
        var turnForm = BlocksManager.Instance.SerializeMyBlocks();
        turnForm.playerId = playerId;
        LoadingScreen.SetProgress(0, "Waiting for others...");
        WebApiHelper.CallApi<string>("POST", $"submit-turn", turnForm, (succ,_) => {
            if(!succ)
            {
                Debug.LogError("Failed to post game status");
                ConfirmBlock();
                return;
            }
            print("Player Blocks posted successfully.");
            StartCoroutine(WaitForOthersAsync());
        });
    }


    IEnumerator WaitForOthersAsync()
    {
        bool isOk = false;
        float fakeProgress = 0;
        while(!isOk)
        {
            yield return new WaitForSeconds(1f);
            if(fakeProgress <= 0.987)
            {
                fakeProgress += 0.033f;
            }
            LoadingScreen.SetProgress(fakeProgress);
            
            // retrieve everyone's blocks
            WebApiHelper.CallApi<ActionsDTO>("GET", $"all-player-actions?playerId={playerId}", 
            (succ, data) => {
                if(!succ || data == null)
                {
                    Debug.Log("Waiting for others... or error..?");
                    return;
                }

                if(_phase != GamePhase.WaitForOthers)
                {
                    // Debug.Log("Not in waiting phase, ignore this response");
                    return;
                }

                print("Everyone's blocks retrieved successfully.");
                _phase = GamePhase.Animation;
                isOk = true;
                Animation(data.actions);
            });
        }
        LoadingScreen.SetProgress(1);

    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actions">Actions of each players</param>
    void Animation(List<ActionDTO> actions)
    {

        // assign actions to each unit
        foreach(var unit in _unitObjs)
        {
            var u = unit.GetComponent<Unit>();
            u.BlockController.ResetBlocks();

            var action = actions.Find(a => a.unit == u.unitId);
            if(action != null)
            {
                u.BlockController.DeserializeBlocks(action.blocks);
            }
        }
        StartCoroutine(WaitForAnimationDone());
    }

    IEnumerator WaitForAnimationDone()
    {
        
        foreach(var unit in _unitObjs)
        {
            unit.GetComponent<Unit>().BlockController.SetEnable(true);
            unit.GetComponent<Unit>().BlockController.Reload();
        }
        yield return new WaitForSeconds(.5f);
        // print("Do animation");
        // _playButtonDummy.onClick?.in();
        // yield return new WaitForSeconds(1f);
        foreach(var unit in _unitObjs)
        {
            unit.GetComponent<Unit>().BlockController.SetEnable(false);
        }

        _playButtonDummy.gameObject.SetActive(true);
        _playButtonDummy.IsPlayed = false;
        while(!_playButtonDummy.IsPlayed)
        {
            yield return null;
        }
        _playButtonDummy.gameObject.SetActive(false);

        // TODO wait until animation done
        yield return new WaitForSeconds(3f);

        // TODO P1: post state 
        // if(playerId == "player1")
        // {

        // 
        WebApiHelper.CallApi<string>("POST", $"turn-result", new GameStatusDTO(), (succ, data) => {
            
        });

        _phase = GamePhase.ArrangeBlocks;
    }


}