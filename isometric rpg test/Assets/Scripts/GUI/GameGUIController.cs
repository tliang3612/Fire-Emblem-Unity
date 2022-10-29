using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameGUIController : GUIController
{
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

    protected override void Awake()
    {
        base.Awake();
        TerrainInfoPanel.SetActive(false);
        UnitInfoPanel.SetActive(false);
    }

    protected override void OnGameStarted(object sender, EventArgs e)
    {
        foreach (OverlayTile tile in tileGrid.TileList)
        {
            tile.TileHighlighted += OnTileHighlighted;
            tile.TileDehighlighted += OnTileDehighlighted;
        }

        OnTurnEnded(sender, e);
    }
    protected override void OnGameEnded(object sender, EventArgs e)
    {
        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1) + " wins!";
    }

    protected override void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as TileGrid).CurrentPlayer is HumanPlayer);

        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1);
    }

    protected override void OnTileDehighlighted(object sender, EventArgs e)
    {
        HideTerrainPanel();
    }
    protected override void OnTileHighlighted(object sender, EventArgs e)
    {
        ShowTerrainPanel(sender as OverlayTile);
    }

    protected override void OnUnitDehighlighted(object sender, EventArgs e)
    {
        HideUnitPanel();
        HideTerrainPanel();
    }
    protected override void OnUnitHighlighted(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        UpdateHpBar(unit);
        StatsText.text = unit.UnitName + "\nHP: " + unit.HitPoints + "/" + unit.TotalHitPoints;
        ShowUnitPanel(unit);
        ShowTerrainPanel(unit.Tile);
    }


    protected override void OnUnitClicked(object sender, EventArgs e)
    {

    }

    protected void ShowUnitPanel(Unit unit)
    {
        UnitInfoPanel.SetActive(true);
        UnitImage.sprite = unit.UnitPortrait;
    }

    protected void ShowTerrainPanel(OverlayTile tile)
    {
        TerrainName.text = tile.TileName;
        TerrainDef.text = tile.DefenseBoost.ToString();
        TerrainAvo.text = tile.AvoidBoost.ToString();

        TerrainInfoPanel.SetActive(true);
    }
    protected  void HideUnitPanel()
    {
        UnitImage.sprite = null;
        UnitInfoPanel.SetActive(false);
    }

    protected void HideTerrainPanel()
    {
        TerrainInfoPanel.SetActive(false);
    }

    protected void UpdateHpBar(Unit unit)
    {
        HpBar.transform.localScale = new Vector3((float)(unit.HitPoints / (float)unit.TotalHitPoints), 1, 1);
    }

    protected void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
}

