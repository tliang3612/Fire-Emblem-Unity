using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGridStateGameOver : TileGridState
{

    public TileGridStateGameOver(TileGrid tileGrid) : base(tileGrid)
    {
    }

    public override TileGridState TransitionState(TileGridState nextState)
    {
        return this;
    }
}



