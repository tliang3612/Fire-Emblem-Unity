using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SummaryUI : MonoBehaviour
{
    public GameObject InfoPanel;
    public Text TurnsPassedText;
    private List<GameObject> InfoPanels = new List<GameObject>();
    private Dictionary<int, int> UnitsDestroyed = new Dictionary<int, int>();

    private int turnsPassed;

    private void Awake()
    {
        FindObjectOfType<TileGrid>().GameStarted += OnGameStarted;
        FindObjectOfType<TileGrid>().TurnEnded += OnTurnEnded; ;
    }

    private void OnTurnEnded(object sender, System.EventArgs e)
    {
        turnsPassed++;
    }

    private void OnGameStarted(object sender, System.EventArgs e)
    {
        
        foreach (var player in FindObjectOfType<TileGrid>().PlayersList)
        {
            UnitsDestroyed.Add(player.PlayerNumber, 0);
        }
    }

    private void OnUnitSpawned(object sender, System.EventArgs e)
    {
        (sender as GameObject).GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;
    }

    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        UnitsDestroyed[(sender as Unit).PlayerNumber] += 1;
    }

    public void UpdateUI()
    {
        var tileGrid = FindObjectOfType<TileGrid>();
        var players = tileGrid.PlayersList.OrderBy(p => p.PlayerNumber).ToList();
        for (int i = players.Count - 1; i >= 0; i--)
        {
            Player player = tileGrid.PlayersList[i];
            var newInfoPanel = Instantiate(InfoPanel);
            newInfoPanel.transform.parent = InfoPanel.transform.parent;
            newInfoPanel.transform.SetSiblingIndex(3);
            newInfoPanel.SetActive(true);

            var playerText = newInfoPanel.transform.Find("PlayerText");
            playerText.GetComponentInChildren<Text>().text = string.Format("Player {0}", player.PlayerNumber);

            var unitsText = newInfoPanel.transform.Find("UnitsText");
            unitsText.GetComponentInChildren<Text>().text = tileGrid.UnitList.FindAll(u => u.PlayerNumber.Equals(player.PlayerNumber)).Count.ToString();

            var unitsDestroyedText = newInfoPanel.transform.Find("UnitsDestroyedText");
            unitsDestroyedText.GetComponentInChildren<Text>().text = UnitsDestroyed[player.PlayerNumber].ToString();

            newInfoPanel.GetComponent<Image>().color = player.Color;

            InfoPanels.Add(newInfoPanel);
        }

        TurnsPassedText.text = string.Format("Turn {0}", (turnsPassed / FindObjectOfType<TileGrid>().PlayersList.Count) + 1);
    }

    public void Cleanup()
    {
        foreach (var panel in InfoPanels)
        {
            Destroy(panel);
        }

        InfoPanels = new List<GameObject>();
    }
}


