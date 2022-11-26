using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Enemy Unit isnt doing confirm mocve poropely 
public class MoveAbility : Ability
{
    public OverlayTile Destination { get; set; }

    private HashSet<OverlayTile> _availableDestinations;
    private List<OverlayTile> _path;
    private HashSet<OverlayTile> _tilesInAttackRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Move";
        IsDisplayableAsButton = false;
    }


    public override IEnumerator Act(TileGrid tileGrid)
    {
        if(CanPerform(tileGrid) && _availableDestinations.Contains(Destination))
        {
            var path = UnitReference.FindPath(Destination, tileGrid);
            UnitReference.Move(path);
            while (UnitReference.IsMoving)
            {
                yield return null;
            }
        }
        
        yield return null;
    }

    public override void Display(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            foreach (var t in _tilesInAttackRange)
                t.MarkAsAttackableTile();

            foreach(var t in _availableDestinations)           
                t.MarkAsReachable();
        }
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HighlightedOnUnit();

        if (CanPerform(tileGrid) && _availableDestinations.Contains(unit.Tile))
        {
            _path = UnitReference.FindPath(unit.Tile, tileGrid);
            TranslateArrows(tileGrid);
        }
    }

    public override void OnUnitDehighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.DeHighlightedOnUnit();
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {       
        if(UnitReference == unit)
        {
            Destination = UnitReference.Tile;
            //We would still call execute so we can run Act
            StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<DisplayActionsAbility>()));
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        if (_availableDestinations.Contains(tile) && UnitReference.IsTileMovableTo(tile))
        {
            Destination = tile;
            StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<DisplayActionsAbility>()));
        }
    }

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.MarkAsHighlighted();

        if (CanPerform(tileGrid) && _availableDestinations.Contains(tile) )
        {
            _path = UnitReference.FindPath(tile, tileGrid);
            TranslateArrows(tileGrid);
        }        
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.MarkAsDeHighlighted();

        if (CanPerform(tileGrid) && _availableDestinations.Contains(tile))
        {
            foreach (var t in _availableDestinations)
                t.MarkAsReachable();
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);

        //play move animation when a human player selects the Unit,
        //we dont want ai to play it because ai instantly selects a move option while the player has to think
        if (tileGrid.CurrentPlayer is HumanPlayer)
        {
            UnitReference.SetState(new UnitStateMoving(UnitReference, Vector2Int.zero));      
        }
        _availableDestinations = UnitReference.GetAvailableDestinations(tileGrid);

        _tilesInAttackRange = UnitReference.GetTilesInAttackRange(_availableDestinations, tileGrid);
    }

    public override void OnRightClick(TileGrid tileGrid)
    {
        UnitReference.SetState(new UnitStateNormal(UnitReference));
        tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<ResetAbility>());
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        foreach (var t in _availableDestinations.Union(_tilesInAttackRange))
            t.UnMark();

        ResetArrows();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.ActionPoints > 0 && UnitReference.MovementPoints > 0 && UnitReference.GetAvailableDestinations(tileGrid).Count > 1;
    }

    private void TranslateArrows(TileGrid tileGrid)
    {
        foreach (var tile in _availableDestinations)
        {
            tile.MarkArrowPath(ArrowTranslator.ArrowDirection.None);
        }

        for (int i = 0; i < _path.Count; i++)
        {
            var previousTile = i > 0 ? _path[i - 1] : UnitReference.Tile;
            var futureTile = i < _path.Count - 1 ? _path[i + 1] : null;

            var arrow = tileGrid.ArrowTranslator.TranslateDirection(previousTile, _path[i], futureTile);
            _path[i].MarkArrowPath(arrow);
        }
    }

    private void ResetArrows()
    {
        _path?.ForEach(t => t.MarkArrowPath(ArrowTranslator.ArrowDirection.None));
    }

    
}


