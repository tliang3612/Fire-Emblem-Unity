using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public TileGrid tileGrid;
    public PhaseTransition phaseTransition;
    public Button NextTurnButton;

    public Image UnitImage;
    public Image HpBar;
    public Text InfoText;
    public Text StatsText;
    public GameObject UnitInfoPanel;

    public GameObject TerrainInfoPanel;
    public Text TerrainName;
    public Text TerrainDef;
    public Text TerrainAvo;

    private void Awake()
    {
        TerrainInfoPanel.SetActive(false);
        UnitInfoPanel.SetActive(false);

        tileGrid.GameStarted += OnGameStarted;
        tileGrid.TurnEnded += OnTurnEnded;
        tileGrid.GameEnded += OnGameEnded;
        tileGrid.UnitAdded += OnUnitAdded;
       
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        foreach (OverlayTile tile in tileGrid.TileList)
        {
            tile.TileHighlighted += OnTileHighlighted;
            tile.TileDehighlighted += OnTileDehighlighted;
        }

        OnTurnEnded(sender, e); 
    }
    private void OnGameEnded(object sender, EventArgs e)
    {
        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1) + " wins!";
        var remainingHP = (sender as TileGrid).UnitList.Where(u => u.PlayerNumber == (sender as TileGrid).CurrentPlayerNumber).Sum(u => u.HitPoints);
    }
    private void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as TileGrid).CurrentPlayer is HumanPlayer);

        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1);
    }


    private void OnTileDehighlighted(object sender, EventArgs e)
    {
        HideTerrainPanel();
    }
    private void OnTileHighlighted(object sender, EventArgs e)
    {

        ShowTerrainPanel(sender as OverlayTile);
    }

    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        OnUnitDehighlighted(sender, EventArgs.Empty);

        if ((sender as Unit).HitPoints <= 0) return;

        UpdateHpBar(sender as Unit);

        OnUnitHighlighted(sender, e);
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        HideUnitPanel();
        HideTerrainPanel();
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        UpdateHpBar(unit);
        StatsText.text = unit.UnitName + "\nHP: " + unit.HitPoints + "/" + unit.TotalHitPoints;
        ShowUnitPanel(unit);
        ShowTerrainPanel(unit.Tile);
    }


    private void OnUnitClicked(object sender, EventArgs e)
    {
        
    }

    private void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.unit);
    }

    private void ShowUnitPanel(Unit unit)
    {
        UnitInfoPanel.SetActive(true);
        UnitImage.sprite = unit.UnitPortrait;
    }

    private void ShowTerrainPanel(OverlayTile tile)
    {
        TerrainName.text = tile.TileName;
        TerrainDef.text = tile.DefenseBoost.ToString();
        TerrainAvo.text = tile.AvoidBoost.ToString();

        TerrainInfoPanel.SetActive(true);
    }
    private void HideUnitPanel()
    {

        UnitImage.sprite = null;
        UnitInfoPanel.SetActive(false);
    }

    private void HideTerrainPanel()
    {
        TerrainInfoPanel.SetActive(false);
    }


    private void RegisterUnit(Unit unit)
    {
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitClicked += OnUnitClicked;
    }

    private void UpdateHpBar(Unit unit)
    {
        HpBar.transform.localScale = new Vector3((float)(unit.HitPoints / (float)unit.TotalHitPoints), 1, 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
}

