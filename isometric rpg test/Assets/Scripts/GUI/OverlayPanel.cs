using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OverlayPanel : GUIPanel
{                                               //center              top left               bottom rleft          bottom right
    private readonly Vector2Int[] quadrants = { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1) };
    //half view point of the camera

    [Header("Menu Options")]
    public GameObject MenuOptionsPanel;
    public Button EndTurnButton;

    [Header("Unit Info")]
    public GameObject UnitInfoPanel;
    public Image UnitImage;
    public Image HpBar;
    public Text StatsText;

    [Header("Terrain Info")]
    public GameObject TerrainInfoPanel;
    public Text TerrainName;
    public Text TerrainDef;
    public Text TerrainAvo;

    [Header("Win Condition")]
    public GameObject WinConditionPanel;
    public Text WinConditionText;

    private TileGrid tileGrid;

    protected override void Start()
    {
        UnitInfoPanel.SetActive(false);
        TerrainInfoPanel.SetActive(false);
        MenuOptionsPanel.SetActive(false);
        WinConditionPanel.SetActive(false);
    }

    
    public void Bind(TileGrid tileGrid)
    {
        this.tileGrid = tileGrid;
        tileGrid.GameStarted += OnGameStarted;
        tileGrid.TurnEnded += OnTurnEnded;
        tileGrid.GameEnded += OnGameEnded;
        tileGrid.LevelLoading += OnLevelLoading;
        tileGrid.RightMouseClicked += OnRightClick;
    }

    public void BindUnit(Unit unit, List<Ability> unitAbilities)
    {
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitClicked += OnUnitClicked;
        unit.UnitSelected += OnUnitSelected;
        unit.UnitDeselected += OnUnitDeselected;

/*        foreach (Ability ability in unitAbilities)
        {
            ability.AbilitySelected += OnAbilitySelected;
            ability.AbilityDeselected += OnAbilityDeselected;
        }*/
    }

    protected virtual void OnLevelLoading(object sender, EventArgs e)
    {
        UnitInfoPanel.SetActive(false);
        TerrainInfoPanel.SetActive(false);
        MenuOptionsPanel.SetActive(false);
        WinConditionPanel.SetActive(false);
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

    public void OnGameEnded(object sender, EventArgs e)
    {

    }

    protected virtual void OnTurnEnded(object sender, EventArgs e)
    {

        EndTurnButton.interactable = ((sender as TileGrid).CurrentPlayer is HumanPlayer);
        //if it is not a human player's turn, block gui input
        if (((sender as TileGrid).CurrentPlayer is not HumanPlayer))
            SetState(GUIState.BlockInput);
        else
            SetState(GUIState.Clear);
        HideMenuOptionsPanel();
    }

    protected virtual void OnTileClicked(object sender, EventArgs e)
    {
        //we dont want to register tile click when mouse is over UI
        if ((sender as OverlayTile).CurrentUnit == null && State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            SetState(GUIState.MenuGUISelection);
            ShowMenuOptionsPanel((sender as OverlayTile));

            HideTerrainPanel();
            HideWinConditionPanel();
            HideUnitPanel();
            tileGrid.GridState = new TileGridStateBlockInput(tileGrid);          
        }       
    }

    protected virtual void OnRightClick(object sender, EventArgs e)
    {
        if (State == GUIState.MenuGUISelection && !EventSystem.current.IsPointerOverGameObject())
        {
            SetState(GUIState.Clear);
            HideMenuOptionsPanel();
            tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
        }
    }

    protected virtual void OnTileDehighlighted(object sender, EventArgs e)
    {
        HideTerrainPanel();
        HideWinConditionPanel();
    }
    protected virtual void OnTileHighlighted(object sender, EventArgs e)
    {
        if (State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            ShowTerrainPanel(sender as OverlayTile);
            ShowWinConditionPanel(sender as OverlayTile);
        }
            
    }

    protected virtual void OnUnitDehighlighted(object sender, EventArgs e)
    {       
        HideUnitPanel();
        HideTerrainPanel();         
    }

    protected virtual void OnUnitHighlighted(object sender, EventArgs e)
    {
        if (State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            var unit = sender as Unit;
            UpdateHpBar(unit);
            StatsText.text = unit.UnitName + "\nHP: " + unit.HitPoints + "/" + unit.TotalHitPoints;
            ShowUnitPanel(unit);
            ShowTerrainPanel(unit.Tile);
            ShowWinConditionPanel(unit.Tile);
        }      
    }

    protected virtual void OnUnitClicked(object sender, EventArgs e)
    {

    }

    protected virtual void OnUnitSelected(object sender, EventArgs e)
    {
        SetState(GUIState.InAbilitySelection);
        HideTerrainPanel();
        HideUnitPanel();
        HideWinConditionPanel();       
    }

    protected virtual void OnUnitDeselected(object sender, EventArgs e)
    {
        SetState(GUIState.Clear);
    }

    private void ShowUnitPanel(Unit unit)
    {
        SetUnitPanelPosition(unit.transform.position);
        UnitInfoPanel.SetActive(true);
        UnitImage.sprite = unit.UnitPortrait;
    }

    private void ShowTerrainPanel(OverlayTile tile)
    {
        SetTerrainPanelPosition(tile.transform.position);
        TerrainName.text = tile.TileName;
        TerrainDef.text = tile.DefenseBoost.ToString();
        TerrainAvo.text = tile.AvoidBoost.ToString();

        TerrainInfoPanel.SetActive(true);
    }

    private void ShowMenuOptionsPanel(OverlayTile tile)
    {
        SetState(GUIState.MenuGUISelection);
        SetMenuOptionsPanelPosition(tile.transform.position);
        MenuOptionsPanel.SetActive(true);
    }

    private void ShowWinConditionPanel(OverlayTile tile)
    {
        SetWinConditionPanelPosition(tile.transform.position);
        WinConditionText.text = String.Format("Enemies Remaining \n {0}", tileGrid.UnitList.Where(u => u.PlayerNumber != tileGrid.CurrentPlayerNumber).Count());
        WinConditionPanel.SetActive(true);
    }

    protected void HideUnitPanel()
    {
        UnitImage.sprite = null;
        UnitInfoPanel.SetActive(false);
    }

    protected void HideTerrainPanel()
    {
        TerrainInfoPanel.SetActive(false);
    }

    protected void HideMenuOptionsPanel()
    {
        MenuOptionsPanel.SetActive(false);
    }

    protected void HideWinConditionPanel()
    {
        WinConditionPanel.SetActive(false);
    }

    protected void UpdateHpBar(Unit unit)
    {
        HpBar.transform.localScale = new Vector3((float)(unit.HitPoints / (float)unit.TotalHitPoints), 1, 1);
    }

    private void SetTerrainPanelPosition(Vector3 position)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(position);
        
        if (worldToScreenPoint.x > halfViewPoint.x)         
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[3];      
        else
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[4];
    }

    private void SetUnitPanelPosition(Vector3 postion)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(postion);

        //if position at at the top left
        if (worldToScreenPoint.y > halfViewPoint.y && worldToScreenPoint.x < halfViewPoint.x)
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[3];
        else
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[2];
    }

    private void SetMenuOptionsPanelPosition(Vector3 postion)
    {        
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(postion);     
       
        if (worldToScreenPoint.x < halfViewPoint.x)
            MenuOptionsPanel.GetComponent<RectTransform>().anchoredPosition = -rightPanelPosition;
        else
            MenuOptionsPanel.GetComponent<RectTransform>().anchoredPosition = rightPanelPosition;

    }

    private void SetWinConditionPanelPosition(Vector3 position)
    {
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(position);

        //if mouse is at the top right quadrant of the screen
        if (worldToScreenPoint.x > halfViewPoint.x && worldToScreenPoint.y > halfViewPoint.y)
            WinConditionPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[4];
        else
        {
            WinConditionPanel.GetComponent<RectTransform>().anchoredPosition = topRightPosition * quadrants[1];
        }
    }

    protected void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
}

