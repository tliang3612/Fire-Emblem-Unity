using System.Linq;
using UnityEngine;

public class GameEndCondition : MonoBehaviour
{
    public GameResult CheckGameEnd(TileGrid tileGrid)
    {
        var playersAlive = tileGrid.UnitList.Select(u => u.PlayerNumber).Distinct().ToList();
        if (playersAlive.Count == 1)
        {
            var wPlayer = tileGrid.PlayersList.First(p => p.PlayerNumber == playersAlive[0]);

            return new GameResult(true, wPlayer);
        }
        return new GameResult(false, null);
    }
}
