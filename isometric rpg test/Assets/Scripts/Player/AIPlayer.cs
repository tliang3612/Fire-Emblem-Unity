using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public override void Initialize(TileGrid tileGrid)
    {
        
    }

    public override void Play(TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateAITurn(tileGrid, this);
        
    }
}
