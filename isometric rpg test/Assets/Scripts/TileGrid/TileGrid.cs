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
    /// Turn ended event is invoked at the end of each turn.
    /// </summary>
    /// 
    public event EventHandler<UnitCreatedEventArgs> UnitAdded;

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

    public int NumberOfPlayers { get { return PlayersList.Count; } }

    public Player CurrentPlayer
    {
        get { return PlayersList.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }

    /// <summary>
    /// GameObject that holds player objects.
    /// </summary>
    public Transform PlayersHolder;
    public bool ShouldStartGameImmediately = true;

    public bool GameFinished { get; private set; }
    public List<Player> PlayersList { get; private set; }
    public List<OverlayTile> TileList { get; private set; }
    public List<Unit> UnitList { get; private set; }
    private List<Unit> PlayableUnits = new List<Unit>();

    public Dictionary<Vector2Int, OverlayTile> Map { get; private set; }

    public Tilemap[] tilemapArr { get; private set; }
    public ArrowTranslator ArrowTranslator;
         
         
    private void Start()
    {
        if (ShouldStartGameImmediately)
        {
            InitializeAndStart();
        }
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

        tilemapArr = gameObject.GetComponentsInChildren<Tilemap>();
        TileList = new List<OverlayTile>();
        Map = new Dictionary<Vector2Int, OverlayTile>();

        //Setup overlay tiles for each tilemap in the tilegrid
        for(int i=0; i < transform.GetComponentsInChildren<Tilemap>().Count(); i++)
        {
            SetUpOverlayTiles(tilemapArr[i]);           
        }
            

        //subscribe a series of event handlers to every tile
        foreach (var tile in TileList)
        {
            tile.TileClicked += OnTileClicked;
            tile.TileHighlighted += OnTileHighlighted;
            tile.TileDehighlighted += OnTileDehighlighted;
               
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
        else
        {
            Debug.LogError("No UnitGenerator script attached to grid");
        }

        if (LevelLoadingDone != null)
            LevelLoadingDone.Invoke(this, EventArgs.Empty);
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
        (sender as Unit).GetComponents<Ability>().ToList().ForEach(a => a.OnUnitDestroyed(this));
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
        
        for (int y = bounds.min.y; y < bounds.max.y; y++)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                var tileLocation = new Vector3Int(x, y, bounds.z);
                var tileKey = new Vector2Int(x, y);

                if (tileMap.HasTile(tileLocation) && !Map.ContainsKey(tileKey))
                {
                    var tile = Instantiate(overlayTilePrefab, overlayTileContainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
                                                                                                    //this prevents the overlayTile to appear inside the tilemap
                    tile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                    tile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder + 1;

                    tile.gridLocation = tileLocation;
                    tile.tileMap = tileMap;

                    //Adds the created overlayTile to the 
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
    public void AddUnit(Unit unit, OverlayTile tile = null, Player ownerPlayer = null)
    {
        UnitList.Add(unit);

        if (ownerPlayer != null)
        {
            unit.PlayerNumber = ownerPlayer.PlayerNumber;
        }
        unit.Initialize();

        //Subscribe events
        unit.UnitClicked += OnUnitClicked;
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitDestroyed += OnUnitDestroyed;
        unit.UnitMoved += OnUnitMoved;

        if (UnitAdded != null)
        {
            UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit));
        }

    }

        
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        if (GameStarted != null)
            GameStarted.Invoke(this, EventArgs.Empty);

        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveStart(this);
        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        foreach(Unit u in PlayableUnits)
        {
            var abilityList = u.GetComponents<Ability>();

            foreach(Ability a in abilityList)
            {
                a.OnTurnStart(this);
            }
            u.OnTurnStart();
        }

        //PlayableUnits.ForEach(u => { u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnStart(this)); u.OnTurnStart(); });
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
            u.GetComponents<Ability>().ToList().ForEach(a => a.OnTurnEnd(this));

        }
        TransitionResult transitionResult = GetComponent<TurnResolver>().ResolveTurn(this);

        PlayableUnits = transitionResult.PlayableUnits;
        CurrentPlayerNumber = transitionResult.NextPlayer.PlayerNumber;

        if (TurnEnded != null)
            TurnEnded.Invoke(this, EventArgs.Empty);

        Debug.Log(string.Format("Player {0} turn", CurrentPlayerNumber));

        playableUnits = PlayableUnits;
        foreach(Unit unit in playableUnits)
        {
            if (unit == null)
            {
                continue;
            }

            var abilities = unit.GetComponents<Ability>();
            foreach(Ability ability in abilities)
            {
                ability.OnTurnStart(this);
            }
            unit.OnTurnStart();
        }
        CurrentPlayer.Play(this);
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

    public List<OverlayTile> GetNeighborTiles(OverlayTile tile, List<OverlayTile> searchableTiles)
    {
        var tempMap = Map;

        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();
        if (searchableTiles.Count > 0)
        {
            foreach (var item in searchableTiles)
            {
                if (!tilesToSearch.ContainsKey(item.gridLocation2D))
                    tilesToSearch.Add(item.gridLocation2D, item);
            }
        }
        else
        {
            tilesToSearch = Map;
        }

        List<OverlayTile> neighbors = new List<OverlayTile>();


        Vector2Int locationToCheck = new Vector2Int();

        //checks left and right neighbors
        for (int i = 1; i >= -1; i -= 2)
        {
            locationToCheck = new Vector2Int(tile.gridLocation.x + i, tile.gridLocation.y);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                var val = tilesToSearch.FirstOrDefault(x => x.Key == locationToCheck).Value;
                if (val.tileMap.GetCellCenterWorld(val.gridLocation).z >= 0)
                {
                    neighbors.Add(tilesToSearch[locationToCheck]);
                }
            }
        }

        //checks top and down neighbors
        for (int i = 1; i >= -1; i -= 2)
        {
            locationToCheck = new Vector2Int(tile.gridLocation.x, tile.gridLocation.y + i);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                var val = tilesToSearch.FirstOrDefault(x => x.Key == locationToCheck).Value;
                if (val.tileMap.GetCellCenterWorld(val.gridLocation).z >= 0)
                {
                    neighbors.Add(tilesToSearch[locationToCheck]);
                }
            }
        }

        return neighbors;
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
