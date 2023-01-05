using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public override void Play(TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
    }
}
