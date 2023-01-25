using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnResolver : MonoBehaviour
{
    public TransitionResult ResolveStart(TileGrid tileGrid)
    {
        var nextPlayerNumber = tileGrid.PlayersList.Min(p => p.PlayerNumber);
        var nextPlayer = tileGrid.PlayersList.Find(p => p.PlayerNumber == nextPlayerNumber);
        List<Unit> allowedUnits = tileGrid.UnitList.FindAll(u => u.PlayerNumber == nextPlayerNumber);

        return new TransitionResult(nextPlayer, allowedUnits);
    }

    public TransitionResult ResolveTurn(TileGrid tileGrid)
    {
        var nextPlayerNumber = (tileGrid.CurrentPlayerNumber + 1) % tileGrid.NumberOfPlayers;
        while (tileGrid.UnitList.FindAll(u => u.PlayerNumber.Equals(nextPlayerNumber)).Count == 0)
        {
            nextPlayerNumber = (nextPlayerNumber + 1) % tileGrid.NumberOfPlayers;
        }

        var nextPlayer = tileGrid.PlayersList.Find(p => p.PlayerNumber == nextPlayerNumber);
        var allowedUnits = tileGrid.UnitList.FindAll(u => u.PlayerNumber == nextPlayerNumber);

        return new TransitionResult(nextPlayer, allowedUnits);
    }
}
