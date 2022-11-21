using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Enemy Unit isnt doing confirm mocve poropely 
public class MoveAbility : Ability
{
    public OverlayTile Destination { get; set; }
    public List<OverlayTile> availableDestinations { get; set; }  
    public List<OverlayTile> path { get; set; }

    private List<OverlayTile> _tilesInAttackRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Move";
        IsDisplayable = false;
    }


    public override IEnumerator Act(TileGrid tileGrid)
    {
        if(CanPerform(tileGrid) && availableDestinations.Contains(Destination))
        {
            var path = UnitReference.FindPath(Destination, tileGrid);
            UnitReference.Move(path);
            while (UnitReference.IsMoving)
            {
                yield return 0;
            }
        }
        
        yield return 0;
    }

    public override void Display(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            _tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());

            availableDestinations.ForEach(t => t.MarkAsReachable());
        }
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        
        if(UnitReference == unit)
        {
            Destination = UnitReference.Tile;
            //We would still call execute so we can run Act
            StartCoroutine(Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<DisplayActionsAbility>())));
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        if (availableDestinations.Contains(tile) && UnitReference.IsTileMovableTo(tile))
        {
            Destination = tile;
            StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<DisplayActionsAbility>())));
        }
    }

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && availableDestinations.Contains(tile) )
        {
            path = UnitReference.FindPath(tile, tileGrid);
            TranslateArrows(tileGrid);       
        }
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && availableDestinations.Contains(tile))
        {
            availableDestinations.ForEach(t => t.MarkAsReachable());
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        //play move animation when a human player selects the Unit,
        //we dont want ai to play it because ai instantly selects a move option while the player has to think
        if(tileGrid.CurrentPlayer is HumanPlayer)
        {
            UnitReference.SetMove();
            base.OnAbilitySelected(tileGrid);
        }
            

        availableDestinations = UnitReference.GetAvailableDestinations(tileGrid);
        _tilesInAttackRange = UnitReference.GetTilesInAttackRange(availableDestinations, tileGrid);
    }

    public override void OnRightClick(TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
        //We call this to inform GUI that the move has been cancelled, which allows the GUI to show 
        OnAbilityDeselected(tileGrid);
        UnitReference.SetState(new UnitStateNormal(UnitReference));
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        availableDestinations.ForEach(t => t.UnMark());
        _tilesInAttackRange.ForEach(t => t.UnMark());
        ResetArrows();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.ActionPoints > 0 && UnitReference.MovementPoints > 0 && UnitReference.GetAvailableDestinations(tileGrid).Count > 1;
    }

    private void TranslateArrows(TileGrid tileGrid)
    {
        foreach (var tile in availableDestinations)
        {
            tile.MarkArrowPath(ArrowTranslator.ArrowDirection.None);
        }

        for (int i = 0; i < path.Count; i++)
        {
            var previousTile = i > 0 ? path[i - 1] : UnitReference.Tile;
            var futureTile = i < path.Count - 1 ? path[i + 1] : null;

            var arrow = tileGrid.ArrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
            path[i].MarkArrowPath(arrow);
        }
    }

    private void ResetArrows()
    {
        path?.ForEach(t => t.MarkArrowPath(ArrowTranslator.ArrowDirection.None));
    }

    
}


