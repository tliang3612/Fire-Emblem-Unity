using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTransition : MonoBehaviour
{
    public Animator phaseTransitionAnim;
    public TileGrid tileGrid;

    private void Awake()
    {
        gameObject.SetActive(false);
        tileGrid.GameStarted += TransitionPhase;
        tileGrid.TurnEnded += TransitionPhase;            
    }


    public void TransitionPhase(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        tileGrid.GridState = new TileGridStateBlockInput(tileGrid);
    
        StartCoroutine(PlayPhaseTransition(sender as TileGrid));        

    }
    IEnumerator PlayPhaseTransition(TileGrid tileGrid)
    {
        if(tileGrid.CurrentPlayer is HumanPlayer)
            phaseTransitionAnim.SetTrigger("PlayerPhase");
        else
            phaseTransitionAnim.SetTrigger("EnemyPhase");

        yield return new WaitForSeconds(3.5f);
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);

        gameObject.SetActive(false);
        tileGrid.StartPlayerTurn();

    }
}
