using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PhaseTransition : MonoBehaviour
{
    public Animator phaseTransitionAnim;
    private Image _background;

    [SerializeField] private Sprite _playerPhaseSprite;
    [SerializeField] private Sprite _enemyPhaseSprite;
    [SerializeField] private Sprite _combatPhaseSprite;
    [SerializeField] private Image _transitionImage;

    public TileGrid tileGrid;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public async Task TransitionPhase(bool isHumanPlayer)
    {
        gameObject.SetActive(true);
        phaseTransitionAnim.SetTrigger("PlayerPhase");

        if (isHumanPlayer)      
            _transitionImage.sprite = _playerPhaseSprite;         
        else
            _transitionImage.sprite = _enemyPhaseSprite;

        await Task.Delay(3500);
    }

    public IEnumerator CombatTransition()
    {
        gameObject.SetActive(true);
        phaseTransitionAnim.SetTrigger("StartBattle");
        yield return new WaitForSeconds(1.5f);
    }

    public void EndCombatTransition()
    {
        phaseTransitionAnim.SetTrigger("EndBattle");
    }

}
