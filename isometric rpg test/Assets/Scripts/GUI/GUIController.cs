using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GUIState
{
    Clear,
    InAbilitySelection,
    InGameGUISelection,
    BlockInput
}
public class GUIController : MonoBehaviour
{
    public GameObject Panel;
    public TileGrid tileGrid;
    protected Camera mainCamera;

    //communicates state between all gui classes
    protected static GUIState State;
    [SerializeField] public GameObject RightPanelHolder;

    protected Vector2 rightPanelPosition;

    protected virtual void Awake()
    {
        State = GUIState.Clear;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rightPanelPosition = RightPanelHolder.GetComponent<RectTransform>().anchoredPosition;
        RightPanelHolder.SetActive(false);

        tileGrid.GameStarted += OnGameStarted;
        tileGrid.TurnEnded += OnTurnEnded;
        tileGrid.GameEnded += OnGameEnded;
        tileGrid.UnitAdded += OnUnitAdded;
    }

    protected virtual void OnGameStarted(object sender, EventArgs e)
    {
        foreach (OverlayTile tile in tileGrid.TileList)
        {
            tile.TileHighlighted += OnTileHighlighted;
            tile.TileDehighlighted += OnTileDehighlighted;
            tile.TileClicked += OnTileClicked;
        }

        OnTurnEnded(sender, e); 
    }

    protected void SetState(GUIState state)
    {
        State = state;
    }

    protected virtual void OnTileClicked(object sender, EventArgs e)
    {
        
    }   
    
    protected virtual void OnGameEnded(object sender, EventArgs e)
    {
        
    }
    protected virtual void OnTurnEnded(object sender, EventArgs e)
    {

    }

    protected virtual void OnTileDehighlighted(object sender, EventArgs e)
    {
        
    }
    protected virtual void OnTileHighlighted(object sender, EventArgs e)
    {
      
    }

    protected virtual void OnUnitDehighlighted(object sender, EventArgs e)
    {
        
    }

    protected virtual void OnUnitHighlighted(object sender, EventArgs e)
    {
        
    }


    protected virtual void OnUnitClicked(object sender, EventArgs e)
    {
        
    }

    protected virtual void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.Unit, e.Abilities);
    }


    protected virtual void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitClicked += OnUnitClicked;
    }
}

