using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public string playerId = "player1";
    public List<UnitObj> unitPrefabs;
    public Transform _worldContainer;

    /* -------------------------------------------------------------------------- */
    [Header("Runtime")]
    public int _countDownTimer = 0;


    [SerializeField]
    public List<GameObject> _objects = new List<GameObject>();
    [SerializeField]
    public List<Unit> _units = new List<Unit>();


    /* -------------------------------------------------------------------------- */

    public enum GamePhase { Init, ArrangeBlocks, WaitForOthers, Animation, GG }
    public GamePhase _phase = GamePhase.Init;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
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

        // Initialize objects
        foreach (var obj in gameStatus.obstacles)
        {
            // GameObject objPrefab = Resources.Load<GameObject>("Obstacles/"+obj.name);
            // if (objPrefab != null)
            // {
            //     var u = Instantiate(objPrefab, new Vector3(obj.x, 0, obj.y), Quaternion.identity);
                
            // }
        }

        // init 
        foreach (var unit in gameStatus.units)
        {
            GameObject unitPrefab = Resources.Load<GameObject>("Units/BasicUnit");
            var u = Instantiate(unitPrefab, _worldContainer).GetComponent<Unit>();
            u.transform.localPosition = new Vector3(unit.x, 0, unit.y);
            u.transform.localRotation = Quaternion.Euler(0, unit.angle, 0);
            u.playerId = unit.playerId;
            if(u.playerId == playerId)
            {
                BlocksManager.Instance.RegisterMyUnit(u);
            }
            u.hp = unit.hp;
            u.fp = unit.fp;
            u.unitId = unit.id.ToString();
            _units.Add(u);
        }

        _phase = GamePhase.ArrangeBlocks;
        _countDownTimer = 30;
    }

    /* -------------------------------------------------------------------------- */

    public void ConfirmBlock()
    {
        _phase = GamePhase.WaitForOthers;
        
        // call api
        WebApiHelper.CallApi<string>("POST", "gameStatus", BlocksManager.Instance.SerializeMyBlocks(), (succ,_) => {
            if(!succ)
            {
                Debug.LogError("Failed to post game status");
                return;
            }
            print("Player Blocks posted successfully.");
            StartCoroutine(WaitForOthersAsync());
        });
    }

    IEnumerator WaitForOthersAsync()
    {
        bool isOk = false;
        while(!isOk)
        {
            yield return new WaitForSeconds(1f);
            
            // retrieve everyone's blocks
            WebApiHelper.CallApi<List<string>>("GET", $"allPlayersActions?playerId={playerId}", 
            (succ, data) => {
                if(!succ || data == null)
                {
                    Debug.LogWarning("Failed to get game status");
                    return;
                }
                print("Everyone's blocks retrieved successfully.");
                _phase = GamePhase.Animation;
                Animation(data);
                isOk = true;
            });
        }

    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionsOfAllPlayers">Actions of each players</param>
    void Animation(List<string> actionsOfAllPlayers)
    {
        // to dtos
        List<ActionDTO> actions = new List<ActionDTO>();
        foreach(var actionsOfEachPlayers in actionsOfAllPlayers)
        {
            ActionsDTO actionsOfUnits = JsonUtility.FromJson<ActionsDTO>(actionsOfEachPlayers);
            actions.AddRange(actionsOfUnits.actions);
        }

        // assign actions to each unit
        foreach(var unit in _units)
        {
            var action = actions.Find(a => a.unit == unit.unitId);
            if(action == null)
            {
                continue;
            }

            unit.BlockController.DeserializeBlocks(action.blocks);
            unit.BlockController.PlayBlocks();
            
            // TODO wait until animation done
            StartCoroutine(WaitForAnimationDone());
        }
    }

    IEnumerator WaitForAnimationDone()
    {
        // TODO wait until animation done
        yield return new WaitForSeconds(3f);
        _phase = GamePhase.ArrangeBlocks;
    }


}