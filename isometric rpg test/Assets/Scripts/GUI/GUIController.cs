using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public GameObject Panel;
    public TileGrid tileGrid;

    protected virtual void Awake()
    {
        tileGrid.GameStarted += OnGameStarted;
        tileGrid.TurnEnded += OnTurnEnded;
        tileGrid.GameEnded += OnGameEnded;
        tileGrid.UnitAdded += OnUnitAdded;
    }

    protected virtual void OnGameStarted(object sender, EventArgs e)
    {
        OnTurnEnded(sender, e); 
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

