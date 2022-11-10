using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameGUIController : GUIController
{
                                                    //top right            top left               bottom right           bottom left
    private readonly Vector2Int[] panelPositions = { new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)};

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

    [SerializeField] GameObject TopRightPanelHolder;
    [SerializeField] private float panelOffSet = 70f;
    [SerializeField] private float lerpDuration = .15f;

    private Vector2 topRightPosition;

    //flags the side that the terrain panel is on. 
    private bool onLeftSide;
    

    protected override void Awake()
    {
        base.Awake();
        TerrainInfoPanel.SetActive(false);
        UnitInfoPanel.SetActive(false);
        topRightPosition = TopRightPanelHolder.GetComponent<RectTransform>().anchoredPosition;
        TopRightPanelHolder.SetActive(false);
    }

    protected override void OnTurnEnded(object sender, EventArgs e)
    {
        EndTurnButton.interactable = ((sender as TileGrid).CurrentPlayer is HumanPlayer);
        //if it is not a human player's turn, block gui input
        if (((sender as TileGrid).CurrentPlayer is not HumanPlayer))
            SetState(GUIState.BlockInput);
        else
            SetState(GUIState.Clear);
        HideMenuOptionsPanel();
    }

    protected override void OnTileClicked(object sender, EventArgs e)
    {
        //we dont want to register tile click when mouse is over UI
        if ((sender as OverlayTile).CurrentUnit == null && State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            SetState(GUIState.InGameGUISelection);
            ShowMenuOptionsPanel((sender as OverlayTile));
            HideTerrainPanel();
            tileGrid.GridState = new TileGridStateBlockInput(tileGrid);
            
        }
        else if(State == GUIState.InGameGUISelection && !EventSystem.current.IsPointerOverGameObject())
        {
            SetState(GUIState.Clear);
            HideMenuOptionsPanel();
            tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
        }
    }

    protected override void OnTileDehighlighted(object sender, EventArgs e)
    {
        HideTerrainPanel();
    }
    protected override void OnTileHighlighted(object sender, EventArgs e)
    {
        if (State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            ShowTerrainPanel(sender as OverlayTile);
            ShowWinConditionPanel(sender as OverlayTile);
        }
            
    }

    protected override void OnUnitDehighlighted(object sender, EventArgs e)
    {       
        HideUnitPanel();
        HideTerrainPanel();         
    }
    protected override void OnUnitHighlighted(object sender, EventArgs e)
    {
        if (State == GUIState.Clear && !EventSystem.current.IsPointerOverGameObject())
        {
            var unit = sender as Unit;
            UpdateHpBar(unit);
            StatsText.text = unit.UnitName + "\nHP: " + unit.HitPoints + "/" + unit.TotalHitPoints;
            ShowUnitPanel(unit);
            ShowTerrainPanel(unit.Tile);
        }      
    }

    protected virtual void OnAbilitySelected(object sender, EventArgs e)
    {
        HideTerrainPanel();
        HideUnitPanel();
        SetState(GUIState.BlockInput);
    }

    protected virtual void OnAbilityDeselected(object sender, EventArgs e)
    {
        //if the ability deselected is displayable, ex a display ability, we don't want to set the gui state to clear
        if(!(sender as Ability).IsDisplayable)
        {
            SetState(GUIState.Clear);
        }
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
        SetState(GUIState.InGameGUISelection);
        SetMenuOptionsPanelPosition(tile.transform.position);
        MenuOptionsPanel.SetActive(true);
    }

    private void ShowWinConditionPanel(OverlayTile tile)
    {
        SetWinConditionPanelPosition(tile.transform.position);
        WinConditionText.text = String.Format("{0} Enemies Remaining", tileGrid.UnitList.Where(u => u.PlayerNumber != tileGrid.CurrentPlayerNumber).Count());
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

    protected void UpdateHpBar(Unit unit)
    {
        HpBar.transform.localScale = new Vector3((float)(unit.HitPoints / (float)unit.TotalHitPoints), 1, 1);
    }

    private void SetTerrainPanelPosition(Vector3 position)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(position);
        //Get the center of the screen
        var halfViewPoint = mainCamera.pixelWidth / 2;
        //Check to see if the the ability's transform.x is greater than the center of the screen
        var panelPositionY = TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition.y;
        if (worldToScreenPoint.x > halfViewPoint && !onLeftSide) //Makes sure that the lerp animation doesn't always play when the mouse is moved
        {
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(topRightPosition.x - panelOffSet, panelPositionY);
            TerrainInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(topRightPosition.x, lerpDuration);
            onLeftSide = true;
        }
        else if(worldToScreenPoint.x < halfViewPoint && onLeftSide)
        {
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(topRightPosition.x - panelOffSet), panelPositionY);
            TerrainInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(-topRightPosition.x, lerpDuration);
            onLeftSide = false;
        }
    }

    private void SetUnitPanelPosition(Vector3 postion)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(postion);
        //Get the center of the screen
        var halfViewPointX = mainCamera.pixelWidth / 2;
        var halfViewPointY = mainCamera.pixelHeight / 2;
        //Check to see if the the ability's transform.x is greater than the center of the screen
        var panelPositionX = UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition.x;
        if (worldToScreenPoint.y > halfViewPointY && worldToScreenPoint.x < halfViewPointX)
        {
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelPositionX - panelOffSet , topRightPosition.y);
            UnitInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(panelPositionX, lerpDuration);
        }
        else
        {
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelPositionX - panelOffSet, -topRightPosition.y);
            UnitInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(panelPositionX, lerpDuration);
        }
    }

    private void SetMenuOptionsPanelPosition(Vector3 postion)
    {        
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(postion);     
        var halfViewPointX = mainCamera.pixelWidth / 2;
        
        var panelPositionY = MenuOptionsPanel.GetComponent<RectTransform>().anchoredPosition.y;

        if (worldToScreenPoint.x < halfViewPointX)
        {
            MenuOptionsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-rightPanelPosition.x, panelPositionY);
        }
        else
        {
            MenuOptionsPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(rightPanelPosition.x, panelPositionY);
        }
    }

    private void SetWinConditionPanelPosition(Vector3 position)
    {
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(position);
        var halfViewPointX = mainCamera.pixelWidth / 2;
        var halfViewPointY = mainCamera.pixelHeight / 2;

        var panelPositionY = WinConditionPanel.GetComponent<RectTransform>().anchoredPosition.y;
        var panelPositionX = WinConditionPanel.GetComponent<RectTransform>().anchoredPosition.x;

        //if mouse is at the top right quadrant of the screen
        if (worldToScreenPoint.x > halfViewPointX && worldToScreenPoint.y > halfViewPointY)
        {
            WinConditionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-topRightPosition.x, topRightPosition.y - panelOffSet);
            WinConditionPanel.GetComponent<RectTransform>().DOAnchorPosY(panelPositionY, lerpDuration);
        }
        else
        {
            WinConditionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-topRightPosition.x, -topRightPosition.y + panelOffSet);
            WinConditionPanel.GetComponent<RectTransform>().DOAnchorPosY(panelPositionY, lerpDuration);
        }
    }

    protected override void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        base.RegisterUnit(unit, unitAbilities);
        foreach (Ability a in unitAbilities)
        {
            a.AbilitySelected += OnAbilitySelected;
            a.AbilityDeselected += OnAbilityDeselected;
        }
    }


    protected void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
}

