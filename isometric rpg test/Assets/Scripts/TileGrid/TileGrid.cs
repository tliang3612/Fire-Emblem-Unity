using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;

    /// <summary>
    /// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
    /// It reacts to user interacting with units or cells, and raises events related to game progress. 
    /// </summary>
public class TileGrid : MonoBehaviour
{
    public OverlayTile overlayTilePrefab;
    public GameObject overlayTileContainer;
    /// <summary>
    /// LevelLoading event is invoked before Initialize method is run.
    /// </summary>
    public event EventHandler LevelLoading;
    /// <summary>
    /// LevelLoadingDone event is invoked after Initialize method has finished running.
    /// </summary>
    public event EventHandler LevelLoadingDone;
    /// <summary>
    /// GameStarted event is invoked at the beggining of StartGame method.
    /// </summary>
    public event EventHandler GameStarted;
    /// <summary>
    /// GameEnded event is invoked when there is a single player left in the game.
    /// </summary>
    public event EventHandler<GameEndedArgs> GameEnded;

    /// <summary>
    /// UnitAdded event is invoked when a unit gets added into the game
    /// </summary> 
    public event EventHandler<UnitCreatedEventArgs> UnitAdded;

    /// <summary>
    /// Turn ended event is invoked at the end of each turn.
    /// </summary> 
    public event EventHandler TurnEnded;

    private TileGridState _gridState;
    public TileGridState GridState
    {
        get
        {
            return _gridState;
        }
        set
        {
            TileGridState nextState;
            if (_gridState != null)
            {
                _gridState.OnStateExit();
                nextState = _gridState.TransitionState(value);
            }
            else
            {
                nextState = value;
            }

            _gridState = nextState;
            _gridState.OnStateEnter();
        }
    }
    public List<Player> PlayersList { get; private set; }
    public int NumberOfPlayers { get { return PlayersList.Count; } }
    public Player CurrentPlayer
    {
        get { return PlayersList.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }
    public Transform PlayersHolder;

    public bool ShouldStartGameImmediately = true;
    public bool TurnInProgress { get; set; }
    public bool GameFinished { get; private set; }
   
    public List<Unit> UnitList { get; private set; }
    private List<Unit> PlayableUnits;

    public Tilemap Tilemap { get; private set; }
    public List<OverlayTile> TileList { get; private set; }
    public Dictionary<Vector2Int, OverlayTile> Map { get; private set; }
   
    //A dictionary that contains a TileBase key, and the TileData associated with that key
    public Dictionary<TileBase, TileData> TileDataMap;
    public List<TileData> TileDataList;
    public ArrowTranslator ArrowTranslator;

    [SerializeField]
    private BattleSystem battleSystem;
    [SerializeField]
    private Camera worldCamera;
    public bool IsBattling = false;

    public bool InSelectionMenu = false;



    private void Start()
    {
        InitializeAndStart();
    }

    public void InitializeAndStart()
    {
        Initialize();
        StartGame();
    }

        
    public void Initialize()
    {

        if (LevelLoading != null)
            LevelLoading.Invoke(this, EventArgs.Empty);

        TileDataMap = new Dictionary<TileBase, TileData>();

        //Initialize TileDataMap
        foreach(var tileData in TileDataList)
        {
            foreach(var tileBase in tileData.Tiles)
            {
                TileDataMap.Add(tileBase, tileData);
            }
        }

        GameFinished = false;

        //Adds two players to PlayersList
        PlayersList = new List<Player>();
        for (int i = 0; i < 2; i++)
        {
            var player = PlayersHolder.GetChild(i).GetComponent<Player>();
            if (player != null && player.gameObject.activeInHierarchy)
            {
                PlayersList.Add(player);
            }
        }

        ArrowTranslator = new ArrowTranslator();

        Tilemap = gameObject.GetComponentInChildren<Tilemap>();
        TileList = new List<OverlayTile>();
        Map = new Dictionary<Vector2Int, OverlayTile>();

        SetUpOverlayTiles(Tilemap);
            

        //subscribe a series of event handlers to every tile
        foreach (var tile in TileList)
        {
            tile.TileClicked += OnTileClicked;
            tile.TileHighlighted += OnTileHighlighted;
            tile.TileDehighlighted += OnTileDehighlighted;
            tile.UnMark();
            tile.GetNeighborTiles(this);
        }

        UnitList = new List<Unit>();

        var unitGenerator = GetComponent<UnitGenerator>();
        if (unitGenerator != null)
        {
            var units = unitGenerator.SpawnUnits(this);
            foreach (var unit in units)
            {
                AddUnit(unit);                    
            }
        }

        battleSystem.BattleOver += EndBattle;

        Debug.Log("Initialized");

        if (LevelLoadingDone != null)
            LevelLoadingDone.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Method is called once, at the begining of the game.
    /// </summary>
    public void StartGame()
    {
        if (GameStarted != null)
            GameStarted.Invoke(this, EventArgs.Empty);

        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveStart(this);
        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        foreach (Unit u in UnitList)
        {
            if (!PlayableUnits.Contains(u))
            {
                u.MarkAsEnemy(PlayersList[u.PlayerNumber]);
            }
            else
            {
                var abilityList = u.GetComponentsInChildren<Ability>();

                foreach (Ability a in abilityList)
                {
                    a.OnTurnStart(this);
                }
            }
            u.OnTurnStart();
        }
        Debug.Log("Game Started");
    }


    private void OnTileDehighlighted(object sender, EventArgs e)
    {
        GridState.OnTileDeselected(sender as OverlayTile);
    }
    private void OnTileHighlighted(object sender, EventArgs e)
    {
        GridState.OnTileSelected(sender as OverlayTile);
    }
    private void OnTileClicked(object sender, EventArgs e)
    {
        GridState.OnTileClicked(sender as OverlayTile);
    }     
    private void OnUnitClicked(object sender, EventArgs e)
    {
        GridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        GridState.OnUnitHighlighted(sender as Unit);
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        GridState.OnUnitDehighlighted(sender as Unit);
    }
      
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        UnitList.Remove(sender as Unit);
        (sender as Unit).GetComponentsInChildren<Ability>().ToList().ForEach(a => a.OnUnitDestroyed(this));
        (sender as Unit).MarkAsDestroyed();
        CheckGameFinished();
    }

    private void OnUnitMoved(object sender, MovementEventArgs e)
    {       
        e.StartingTile.CurrentUnit = null;
        e.StartingTile.IsBlocked = false;

        (sender as Unit).Tile = e.DestinationTile;
        e.DestinationTile.CurrentUnit = (sender as Unit);
        e.DestinationTile.IsBlocked = true;
        
        CheckGameFinished();
    }

    private void SetUpOverlayTiles(Tilemap tileMap)
    {
        //limits of the current tilemap
        BoundsInt bounds = tileMap.cellBounds;
        
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var tileLocation = new Vector3Int(x, y, 0);
                var tileKey = new Vector2Int(x, y);

                if (tileMap.HasTile(tileLocation) && !Map.ContainsKey(tileKey))
                {
                    var tile = Instantiate(overlayTilePrefab, overlayTileContainer.transform);

                    tile.InitializeTile(tileMap, tileLocation, TileDataMap);

                    TileList.Add(tile);
                    Map.Add(tileKey, tile);
                }
            }
        }           
    }

    /// <summary>
    /// Adds unit to the game
    /// </summary>
    /// <param name="unit">Unit to add</param>
    public void AddUnit(Unit unit)
    {
        UnitList.Add(unit);
        unit.Initialize();

        //Subscribe events
        unit.UnitClicked += OnUnitClicked;
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitDestroyed += OnUnitDestroyed;
        unit.UnitMoved += OnUnitMoved;

        if (UnitAdded != null)
        {
            UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit, unit.GetComponentsInChildren<Ability>().ToList()));
        }

    }

        
    
    public void StartPlayerTurn()
    {
        TurnInProgress = true;
        CurrentPlayer.Play(this);
    }

    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        GridState = new TileGridStateBlockInput(this);
        bool isGameFinished = CheckGameFinished();
        if (isGameFinished)
        {            
            return;
        }

        var playableUnits = PlayableUnits;
        foreach (Unit u in playableUnits)
        {
            if (u == null)
            {
                continue;
            }

            u.OnTurnEnd();
            u.GetComponentsInChildren<Ability>().ToList().ForEach(a => a.OnTurnEnd(this));

        }
        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveTurn(this);

        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        if (TurnEnded != null)
            TurnEnded.Invoke(this, EventArgs.Empty);

        playableUnits = PlayableUnits;

        foreach (Unit u in UnitList)
        {
            if (u == null)
                continue;

            if (!playableUnits.Contains(u))
            {
                u.MarkAsEnemy(PlayersList[u.PlayerNumber]);
            }
            else
            {
                var abilityList = u.GetComponentsInChildren<Ability>();

                foreach (Ability a in abilityList)
                {
                    a.OnTurnStart(this);
                }
                u.UnMark();
            }
            u.OnTurnStart();          
        }
    }

    public List<Unit> GetCurrentPlayerUnits()
    {
        return PlayableUnits;
    }
    public List<Unit> GetEnemyUnits(Player player)
    {
        return UnitList.FindAll(u => u.PlayerNumber != player.PlayerNumber);
    }
    public List<Unit> GetPlayerUnits(Player player)
    {
        return UnitList.FindAll(u => u.PlayerNumber == player.PlayerNumber);
    }

    public bool CheckGameFinished()
    {
        var gameResult = GetComponent<GameEndCondition>().CheckGameEnd(this);

        if (gameResult.GameOver)
        {
            GridState = new TileGridStateGameOver(this);

            GameFinished = true;

            if (GameEnded != null)
            {
                GameEnded.Invoke(this, new GameEndedArgs(gameResult));
            }

            foreach (Unit unit in UnitList)
            {
                if (unit.HitPoints > 0) unit.UnMark();
            }

        }

        return GameFinished;
    }

    //Euclidean Distance (x,y) = sqrt((x1-x2)^2 + (y1-y2)^2)
    public int GetManhattenDistance(OverlayTile start, OverlayTile other)
    {
        return Mathf.Abs(start.gridLocation.x - other.gridLocation.x) + Mathf.Abs(start.gridLocation.y - other.gridLocation.y);
    }

    public void StartBattle(Unit attacker, Unit defender, BattleEvent battleEvent)
    {
        GridState = new TileGridStateBlockInput(this);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle(attacker, defender, battleEvent);
        IsBattling = true;
    }

    public void EndBattle(object sender, EventArgs e)
    {
        GridState = new TileGridStateWaitingForInput(this);
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        IsBattling = false;

    }
}

public class GameEndedArgs : EventArgs
{
    public GameResult gameResult { get; set; }

    public GameEndedArgs(GameResult result)
    {
        gameResult = result;
    }
}
