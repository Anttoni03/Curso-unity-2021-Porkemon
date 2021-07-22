using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private BattleDialogueBox battleDialogueBox;


    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerUnit.SetUpPorkemon();
        playerHUD.SetPorkemonData(playerUnit.Porkemon);

        enemyUnit.SetUpPorkemon();
        enemyHUD.SetPorkemonData(enemyUnit.Porkemon);
        StartCoroutine(battleDialogueBox.SetDialogue($"Un porkémon salvaje apareció. Es un {enemyUnit.Porkemon.Base.Name}!"));
    }
}
