using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGridStateAITurn : TileGridState
{
    private AIPlayer AIPlayer;

    public TileGridStateAITurn(TileGrid tileGrid, AIPlayer AIPlayer) : base(tileGrid)
    {
        this.AIPlayer = AIPlayer;
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

}
