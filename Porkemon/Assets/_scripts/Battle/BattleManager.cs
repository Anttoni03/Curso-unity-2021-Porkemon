using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerSelectMove,
    EnemyMove,
    Busy,
    PartySelectScreen
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogueBox battleDialogueBox;

    [SerializeField] private PartyHUD partyHUD;

    public BattleState state;

    public event Action<bool> OnBattleFinish;

    private PorkemonParty playerParty;

    private Porkemon wildPorkemon;

    public void HandleStartBattle(PorkemonParty playerParty, Porkemon wildPorkemon)
    {
        this.playerParty = playerParty;
        this.wildPorkemon = wildPorkemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {

        state = BattleState.StartBattle;

        playerUnit.SetUpPorkemon(playerParty.GetFirstNonFaintedPorkemon());
        playerHUD.SetPorkemonData(playerUnit.Porkemon);

        battleDialogueBox.SetPorkemonsMovements(playerUnit.Porkemon.Moves);

        enemyUnit.SetUpPorkemon(wildPorkemon);
        enemyHUD.SetPorkemonData(enemyUnit.Porkemon);

        partyHUD.InitPartyHUD();

        yield return battleDialogueBox.SetDialog($"Un porkémon salvaje apareció. Es un {enemyUnit.Porkemon.Base.Name}!");

        if (enemyUnit.Porkemon.Speed > playerUnit.Porkemon.Speed)
        {
            //StartCoroutine(battleDialogueBox.SetDialog("El enemigo ataca primero"));
            EnemyAction();
        }
        else
        {
            PlayerAction();
        }
    }

    void PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        StartCoroutine(battleDialogueBox.SetDialog("Selecciona una acción"));
        battleDialogueBox.ToggleDialogText(true);
        battleDialogueBox.ToggleActions(true);
        battleDialogueBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogueBox.SelectedAction(currentSelectedAction);
    }

    void PlayerMovement()
    {
        state = BattleState.PlayerSelectMove;
        battleDialogueBox.ToggleDialogText(false);
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleMovements(true);
        currentSelectedAction = 0;
        battleDialogueBox.SelectedMovement(currentSelectedMovement, playerUnit.Porkemon.Moves[currentSelectedMovement]);
    }

    IEnumerator EnemyAction()
    {
        state = BattleState.EnemyMove;
        Move move = enemyUnit.Porkemon.RandomMove();
        move.PP--;
        yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} ha usado {move.Base.Name}.");

        var oldHpValue = playerUnit.Porkemon.HP;

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayReceiveAttackAnimation();

        var damageDesc = playerUnit.Porkemon.ReceiveDamage(enemyUnit.Porkemon, move);
        playerHUD.UpdatePokemonData(oldHpValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} se ha debilitado");
            playerUnit.PlayFaintAnimation();
            
            yield return new WaitForSeconds(1.5f);

            var nextPorkemon = playerParty.GetFirstNonFaintedPorkemon();
            if (nextPorkemon == null)
            {
                //No quedan porkémons sanos
                OnBattleFinish(false);
            }
            else
            {
                //Quedan porkémons sanos. Sacar otro
                playerUnit.SetUpPorkemon(nextPorkemon);
                playerHUD.SetPorkemonData(nextPorkemon);
                battleDialogueBox.SetPorkemonsMovements(nextPorkemon.Moves);
                yield return battleDialogueBox.SetDialog($"Adelante {nextPorkemon.Base.Name}!");

                PlayerAction();
            }
        }
        else
        {
            PlayerAction();
        }

    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (battleDialogueBox.isWriting)
        {
            return;
        }

        if (state == BattleState.PlayerSelectAction)
        {
            HandlePlayerActionSelection();
        }
        else if (state == BattleState.PlayerSelectMove)
        {
            HandlePlayerMovementSelection();
        }
        else if (state == BattleState.PartySelectScreen)
        {
            HandlePlayerPartySelection();
        }
    }

    private int currentSelectedAction;
    private float timeSinceLastClick;
    public float TimeBetweenClicks = .15f;
    
    private void HandlePlayerActionSelection()
    {
        if (timeSinceLastClick < TimeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;

            currentSelectedAction = (currentSelectedAction + 2) % 4;

            battleDialogueBox.SelectedAction(currentSelectedAction);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedAction = (currentSelectedAction + 1) % 2 + 2 * Mathf.FloorToInt(currentSelectedAction / 2);
            battleDialogueBox.SelectedAction(currentSelectedAction);
        }
        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            if (currentSelectedAction == 0)
            {
                PlayerMovement();
            }
            else if (currentSelectedAction == 1)
            {
                OpenPartySelectionScreen();
                //TODO: Implementar los porkémon
            }
            else if (currentSelectedAction == 2)
            {
                //TODO: Implementar mochila
            }
            else if (currentSelectedAction == 3)
            {
                //TODO: Implementar la huida
            }
        }
    }

    private int currentSelectedMovement;
    
    void HandlePlayerMovementSelection()
    {
        if (timeSinceLastClick < TimeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectedMovement = currentSelectedMovement;
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            if (currentSelectedMovement >= playerUnit.Porkemon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            battleDialogueBox.SelectedMovement(currentSelectedMovement, playerUnit.Porkemon.Moves[currentSelectedMovement]);
        } 
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectedMovement = currentSelectedMovement;
            if (currentSelectedMovement <= 1)
            {
                currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2*Mathf.FloorToInt(currentSelectedMovement/2);
            }

            if (currentSelectedMovement >= playerUnit.Porkemon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            battleDialogueBox.SelectedMovement(currentSelectedMovement, playerUnit.Porkemon.Moves[currentSelectedMovement]);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            battleDialogueBox.ToggleMovements(false);
            battleDialogueBox.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMovement());
        }

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            PlayerAction();
        }
    }

    IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.Porkemon.Moves[currentSelectedMovement];
        move.PP--;
        yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} ha usado {move.Base.Name}");

        var oldHpValue = playerUnit.Porkemon.HP;

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        enemyUnit.PlayReceiveAttackAnimation();

        var damageDesc = enemyUnit.Porkemon.ReceiveDamage(playerUnit.Porkemon, move);
        enemyHUD.UpdatePokemonData(oldHpValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} se ha debilitado.");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            
            OnBattleFinish(true);
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critical > 1f)
        {
            yield return battleDialogueBox.SetDialog("¡Un golpe crítico!");
        }
        if (desc.Type > 1)
        {
            yield return battleDialogueBox.SetDialog("¡Es súper efectivo!");
        }
        else if (desc.Type < 1)
        {
            yield return battleDialogueBox.SetDialog("No es muy efectivo.");
        }
    }

    void OpenPartySelectionScreen()
    {
        state = BattleState.PartySelectScreen;
        partyHUD.SetPartyData(playerParty.Porkemons);
        partyHUD.gameObject.SetActive(true);
        currentSelectedPorkemon = 0;
        for (int i = 0; i < playerParty.Porkemons.Count; i++)
        {
            if (playerUnit.Porkemon == playerParty.Porkemons[i])
            {
                currentSelectedPorkemon = i;
            }
        }

        /*if (Input.GetAxisRaw("Cancel") != 0)
        {
            PlayerAction();
        }*/                                     //Borrar???
    }

    private int currentSelectedPorkemon;
    void HandlePlayerPartySelection()
    {
        if (timeSinceLastClick < TimeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPorkemon -= 2*(int)Input.GetAxisRaw("Vertical");
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPorkemon += (int)Input.GetAxisRaw("Horizontal");
        }

        currentSelectedPorkemon = Mathf.Clamp(currentSelectedPorkemon, 0, playerParty.Porkemons.Count - 1);
        partyHUD.UpdateSelectedPorkemon(currentSelectedPorkemon);

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            var selectedPorkemon = playerParty.Porkemons[currentSelectedPorkemon];
            if (selectedPorkemon.HP <= 0)
            {
                partyHUD.SetMessage("No puedes sacar a un porkémon debilitado");
                return;
            }
            else if (selectedPorkemon == playerUnit.Porkemon)
            {
                partyHUD.SetMessage("Tu porkémon ya está en combate!");
                return;
            }
            StartCoroutine(SwitchPorkemon(selectedPorkemon));
        }

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            partyHUD.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    IEnumerator SwitchPorkemon(Porkemon newPorkemon)
    {
        yield return battleDialogueBox.SetDialog($"Vuelve, {playerUnit.Porkemon.Base.Name}!");
        playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);

        playerUnit.SetUpPorkemon(newPorkemon);
        playerHUD.SetPorkemonData(newPorkemon);

        battleDialogueBox.SetPorkemonsMovements(newPorkemon.Moves);
        yield return battleDialogueBox.SetDialog($"Adelante, {newPorkemon.Base.Name}");

        partyHUD.gameObject.SetActive(false);
        state = BattleState.Busy;
        StartCoroutine(EnemyAction());
    }
}
