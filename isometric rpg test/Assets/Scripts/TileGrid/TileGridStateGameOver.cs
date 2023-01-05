using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileGridStateGameOver : TileGridState
{

    public TileGridStateGameOver(TileGrid tileGrid) : base(tileGrid)
    {
        SceneManager.LoadScene(1);
    }

    public override TileGridState TransitionState(TileGridState nextState)
    {
        return this;
    }
}



