using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogueBox battleDialogueBox;

    public BattleState state;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {

        state = BattleState.StartBattle;

        playerUnit.SetUpPorkemon();
        playerHUD.SetPorkemonData(playerUnit.Porkemon);

        battleDialogueBox.SetPorkemonsMovements(playerUnit.Porkemon.Moves);

        enemyUnit.SetUpPorkemon();
        enemyHUD.SetPorkemonData(enemyUnit.Porkemon);

        yield return battleDialogueBox.SetDialog($"Un porkémon salvaje apareció. Es un {enemyUnit.Porkemon.Base.Name}!");

        if (enemyUnit.Porkemon.Speed > playerUnit.Porkemon.Speed)
        {
            StartCoroutine(battleDialogueBox.SetDialog("El enemigo ataca primero"));
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
        state = BattleState.PlayerMove;
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
        yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} ha usado {move.Base.Name}.");

        bool porkemonFainted = playerUnit.Porkemon.ReceiveDamage(enemyUnit.Porkemon, move);

        playerHUD.UpdatePokemonData();

        if (porkemonFainted)
        {
            yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} se ha debilitado");
        }
        else
        {
            PlayerAction();
        }

    }

    private void Update()
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
        else if (state == BattleState.PlayerMove)
        {
            HandlePlayerMovementSelection();
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

            currentSelectedAction = (currentSelectedAction + 1) % 2;

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
                currentSelectedMovement = (currentSelectedMovement + 1) % 2;
            }
            else
            {
                currentSelectedMovement = (currentSelectedMovement - 1) % 2 + 2;
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
    }

    IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.Porkemon.Moves[currentSelectedMovement];
        yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} ha usado {move.Base.Name}");

        bool pokemonFainted = enemyUnit.Porkemon.ReceiveDamage(playerUnit.Porkemon, move);

        enemyHUD.UpdatePokemonData();

        if (pokemonFainted)
        {
            yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} se ha debilitado.");
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
    }
}