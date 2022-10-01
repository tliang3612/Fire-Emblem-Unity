using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public TileGrid tileGrid;
    public Button NextTurnButton;

    public Image UnitImage;
    public Text InfoText;
    public Text StatsText;
    public GameObject UnitInfoPanel;

   private void Awake()
    {
        UnitInfoPanel.SetActive(false);
        tileGrid.GameStarted += OnGameStarted;
        tileGrid.TurnEnded += OnTurnEnded;
        tileGrid.GameEnded += OnGameEnded;
        tileGrid.UnitAdded += OnUnitAdded;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        OnTurnEnded(sender, e);
    }

    private void OnGameEnded(object sender, EventArgs e)
    {
        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1) + " wins!";
        var remainingHP = (sender as TileGrid).UnitList.Where(u => u.PlayerNumber == (sender as TileGrid).CurrentPlayerNumber).Sum(u => u.HitPoints);
    }
    private void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as TileGrid).CurrentPlayer is Player);

        InfoText.text = "Player " + ((sender as TileGrid).CurrentPlayerNumber + 1);
    }
    private void OnTileDehighlighted(object sender, EventArgs e)
    {
        StatsText.text = "";
    }
    private void OnTileHighlighted(object sender, EventArgs e)
    {
        //UnitImage.color = Color.gray;
    }
    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        if (!(tileGrid.CurrentPlayer is Player)) return;
        OnUnitDehighlighted(sender, EventArgs.Empty);

        if ((sender as Unit).HitPoints <= 0) return;

        OnUnitHighlighted(sender, e);
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        StatsText.text = "";
        UnitImage.sprite = null;
        UnitInfoPanel.SetActive(false);
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        StatsText.text = unit.UnitName + "\nHit Points: " + unit.HitPoints + "/" + unit.TotalHitPoints + "\nAttack: " + unit.AttackFactor + "\nDefence: " + unit.DefenceFactor + "\nRange: " + unit.AttackRange;
        UnitInfoPanel.SetActive(true);
        UnitImage.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        
    }


    private void OnUnitClicked(object sender, EventArgs e)
    {
        //var unit = sender as Unit
        //Lock unit prefab that was instantiated
        
    }

    private void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.unit);
    }


    private void RegisterUnit(Unit unit)
    {
        unit.UnitHighlighted += OnUnitHighlighted;
        unit.UnitDehighlighted += OnUnitDehighlighted;
        unit.UnitAttacked += OnUnitAttacked;
        unit.UnitClicked += OnUnitClicked;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }
}

