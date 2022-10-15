using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, PlayerAction, EnemyAction, Busy}
    BattleUnit playerUnit;
    BattleSystemHUD playerHUD;

    BattleUnit enemyUnit;
    BattleSystemHUD enemyHUD;

    BattleState battleState;

    private void Start()
    {
        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.Setup();
        playerHUD.SetData(playerUnit.unit);
        enemyUnit.Setup();
        enemyHUD.SetData(enemyUnit.unit);

        PlayerAction();
    }

    void PlayerAction()
    {
        battleState = BattleState.PlayerAction;

        PerformPlayerMove();

    }

    

    private IEnumerator PerformPlayerMove()
    {
        var damageDetails = playerUnit.unit.AttackHandler(enemyUnit.unit);

        yield return enemyHUD.UpdateHP();

        if (!damageDetails.IsDead)
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove()
    {
        battleState = BattleState.EnemyAction;

        var damageDetails = enemyUnit.unit.AttackHandler(playerUnit.unit);

        yield return playerHUD.UpdateHP();
        
        if(damageDetails.IsDead)
        {
            //play death animation
            //end battle scene
        }
        else
        {
            //end battle scene
        }
    }

    IEnumerator HandleDamageDetails(DamageDetails details)
    {
        if(details.Critical > 1f)
        {
            //play crit animation
            yield return 0;
        }
        if(details.IsDead)
        {
            //play death animation
        }
    }

    

}
