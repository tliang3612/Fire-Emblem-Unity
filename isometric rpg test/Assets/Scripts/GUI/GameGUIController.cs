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
    public GameObject MenuOptionsPanel;
    public Button EndTurnButton;

    public GameObject UnitInfoPanel;
    public Image UnitImage;
    public Image HpBar;
    public Text StatsText;
    
    public GameObject TerrainInfoPanel;
    public Text TerrainName;
    public Text TerrainDef;
    public Text TerrainAvo;

    [SerializeField] GameObject BottomLeftPanelHolder;
    [SerializeField] private float panelOffSet = 70f;
    [SerializeField] private float lerpDuration = .15f;

    private Vector2 bottomLeftPanelPosition;

    //flags the side that the terrain panel is on. 
    private bool onLeftSide;
    

    protected override void Awake()
    {
        base.Awake();
        TerrainInfoPanel.SetActive(false);
        UnitInfoPanel.SetActive(false);
        bottomLeftPanelPosition = BottomLeftPanelHolder.GetComponent<RectTransform>().anchoredPosition;
        BottomLeftPanelHolder.SetActive(false);
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
    {                                                                                 //we dont want to register tile click when mouse is over UI
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
            ShowTerrainPanel(sender as OverlayTile);
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

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        HideTerrainPanel();
        HideUnitPanel();
        SetState(GUIState.BlockInput);
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        SetState(GUIState.Clear);
    }

    protected void ShowUnitPanel(Unit unit)
    {
        SetUnitPanelPosition(unit.transform.position);
        UnitInfoPanel.SetActive(true);
        UnitImage.sprite = unit.UnitPortrait;
    }

    protected void ShowTerrainPanel(OverlayTile tile)
    {
        SetTerrainPanelPosition(tile.transform.position);
        TerrainName.text = tile.TileName;
        TerrainDef.text = tile.DefenseBoost.ToString();
        TerrainAvo.text = tile.AvoidBoost.ToString();

        TerrainInfoPanel.SetActive(true);
    }

    protected void ShowMenuOptionsPanel(OverlayTile tile)
    {
        SetState(GUIState.InGameGUISelection);
        SetMenuOptionsPanelPosition(tile.transform.position);
        MenuOptionsPanel.SetActive(true);
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

    private void SetTerrainPanelPosition(Vector3 postion)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint(postion);
        //Get the center of the screen
        var halfViewPoint = mainCamera.pixelWidth / 2;
        //Check to see if the the ability's transform.x is greater than the center of the screen
        var panelPositionY = TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition.y;
        if (worldToScreenPoint.x > halfViewPoint && !onLeftSide) //Makes sure that the lerp animation doesn't always play when the mouse is moved
        {
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(bottomLeftPanelPosition.x - panelOffSet, panelPositionY);
            TerrainInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(bottomLeftPanelPosition.x, lerpDuration);
            onLeftSide = true;
        }
        else if(worldToScreenPoint.x < halfViewPoint && onLeftSide)
        {
            TerrainInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(bottomLeftPanelPosition.x - panelOffSet), panelPositionY);
            TerrainInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(-bottomLeftPanelPosition.x, lerpDuration);
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
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelPositionX - panelOffSet , bottomLeftPanelPosition.y);
            UnitInfoPanel.GetComponent<RectTransform>().DOAnchorPosX(panelPositionX, lerpDuration);
        }
        else
        {
            UnitInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelPositionX - panelOffSet, -bottomLeftPanelPosition.y);
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

