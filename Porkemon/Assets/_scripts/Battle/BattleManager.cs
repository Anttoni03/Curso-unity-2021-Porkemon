using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;


public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemsSelectScreen,
    FinishBattle,
    LoseTurn
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogueBox battleDialogueBox;

    [SerializeField] private PartyHUD partyHUD;
    [SerializeField] private float TimeBetweenClicks = .15f;
    [SerializeField] private GameObject porkeball;

    public BattleState state;

    public event Action<bool> OnBattleFinish;

    private PorkemonParty playerParty;

    private Porkemon wildPorkemon;

    private int currentSelectedPorkemon;

    private int currentSelectedAction;
    private float timeSinceLastClick;
    private int currentSelectedMovement;

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

        battleDialogueBox.SetPorkemonsMovements(playerUnit.Porkemon.Moves);

        enemyUnit.SetUpPorkemon(wildPorkemon);

        partyHUD.InitPartyHUD();

        yield return battleDialogueBox.SetDialog($"Un porkémon salvaje apareció. Es un {enemyUnit.Porkemon.Base.Name}!");

        if (enemyUnit.Porkemon.Speed > playerUnit.Porkemon.Speed)
        {
            battleDialogueBox.ToggleDialogText(true);
            battleDialogueBox.ToggleMovements(false);
            battleDialogueBox.ToggleActions(false);
            //StartCoroutine(battleDialogueBox.SetDialog("El enemigo ataca primero"));
            yield return PerformEnemyMovement();
        }
        else
        {
            PlayerActionSelection();
        }
    }

    void BattleFinish(bool playerHasWon)
    {
        state = BattleState.FinishBattle;
        OnBattleFinish(playerHasWon);
    }

    void PlayerActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(battleDialogueBox.SetDialog("Selecciona una acción"));
        battleDialogueBox.ToggleDialogText(true);
        battleDialogueBox.ToggleActions(true);
        battleDialogueBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogueBox.SelectedAction(currentSelectedAction);
    }

    void PlayerMovementSelection()
    {
        state = BattleState.MovementSelection;
        battleDialogueBox.ToggleDialogText(false);
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleMovements(true);
        currentSelectedAction = 0;
        battleDialogueBox.SelectedMovement(currentSelectedMovement, playerUnit.Porkemon.Moves[currentSelectedMovement]);
    }

    void OpenPartySelectionScreen()
    {
        state = BattleState.PartySelectScreen;
        partyHUD.SetPartyData(playerParty.Porkemons);
        partyHUD.gameObject.SetActive(true);
        //currentSelectedPorkemon = playerParty.GetFirstNonFaintedPorkemon(playerUnit.Porkemon)
        //Una linea más
    }

    void OpenInventoryScreen()
    {
        //TODO: Inventario e ítems
        print("Abrir inventario");
        StartCoroutine(ThrowPorkeball());
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (battleDialogueBox.isWriting)
        {
            return;
        }

        if (state == BattleState.ActionSelection)
        {
            HandlePlayerActionSelection();
        }
        else if (state == BattleState.MovementSelection)
        {
            HandlePlayerMovementSelection();
        }
        else if (state == BattleState.PartySelectScreen)
        {
            HandlePlayerPartySelection();
        }
        else if (state == BattleState.LoseTurn)
        {
            StartCoroutine(PerformEnemyMovement());
        }
    }

    
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
                PlayerMovementSelection();
            }
            else if (currentSelectedAction == 1)
            {
                OpenPartySelectionScreen();
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
            PlayerActionSelection();
        }
    }

    void HandlePlayerPartySelection()
    {
        if (timeSinceLastClick < TimeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPorkemon -= 2 * (int)Input.GetAxisRaw("Vertical");
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
            PlayerActionSelection();
        }
    }

    IEnumerator PerformPlayerMovement()
    {
        state = BattleState.PerformMovement;
        Move move = playerUnit.Porkemon.Moves[currentSelectedMovement];
        if (move.PP <= 0)
        {
            yield break;
        }

        yield return RunMovement(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMovement)
        {
            StartCoroutine(PerformEnemyMovement());
        }
    }

    IEnumerator PerformEnemyMovement()
    {
        state = BattleState.PerformMovement;
        Move move = enemyUnit.Porkemon.RandomMove();
        if (move.PP <= 0)
        {
            yield break;
        }

        yield return RunMovement(enemyUnit, playerUnit, move);
        if (state == BattleState.PerformMovement)
        {
            PlayerActionSelection();
        }
    }

    IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
    {
        move.PP--;
        yield return battleDialogueBox.SetDialog($"{attacker.Porkemon.Base.Name} ha usado {move.Base.Name}");

        var oldHpValue = target.Porkemon.HP;

        attacker.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        target.PlayReceiveAttackAnimation();

        var damageDesc = target.Porkemon.ReceiveDamage(attacker.Porkemon, move);
        yield return target.Hud.UpdatePokemonData(oldHpValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return battleDialogueBox.SetDialog($"{target.Porkemon.Base.Name} se ha debilitado.");
            target.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);

            CheckForBattleFinish(target);
        }
    }

    void CheckForBattleFinish(BattleUnit faitedUnit)
    {
        if (faitedUnit.IsPlayer)
        {
            var nexPorkemon = playerParty.GetFirstNonFaintedPorkemon();
            if (nexPorkemon != null)
                OpenPartySelectionScreen();
            else
                BattleFinish(false);
        }
        else
        {
            BattleFinish(true);
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

    IEnumerator SwitchPorkemon(Porkemon newPorkemon)
    {
        if (playerUnit.Porkemon.HP > 0)
        {
        yield return battleDialogueBox.SetDialog($"Vuelve, {playerUnit.Porkemon.Base.Name}!");
        playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);
        }

        playerUnit.SetUpPorkemon(newPorkemon);
        battleDialogueBox.SetPorkemonsMovements(newPorkemon.Moves);

        yield return battleDialogueBox.SetDialog($"Adelante, {newPorkemon.Base.Name}");

        partyHUD.gameObject.SetActive(false);
        state = BattleState.Busy;
        StartCoroutine(PerformEnemyMovement());
    }

    IEnumerator ThrowPorkeball()
    {
        state = BattleState.Busy;
        yield return battleDialogueBox.SetDialog($"Has lanzado una {(porkeball.name).ToLower()}");

        var porkeballInst = Instantiate(porkeball, playerUnit.transform.position - 
            new Vector3(2,2), Quaternion.identity);

        var porkeballSpt = porkeballInst.GetComponent<SpriteRenderer>();
        yield return porkeballSpt.transform.DOLocalJump(enemyUnit.transform.position + 
            new Vector3(0,2),2,1,.6f).WaitForCompletion();

        yield return enemyUnit.PlayCapturedAnimation();

        yield return porkeballSpt.transform.DOLocalMoveY(enemyUnit.transform.position.y - 1.6f,.2f).WaitForCompletion();

        var numberOfShakes = TryToCatchPorkemon(enemyUnit.Porkemon);
        for (int i = 0; i < Mathf.Min(numberOfShakes,3); i++)
        {
            yield return new WaitForSeconds(.4f);
            yield return porkeballSpt.transform.DOPunchRotation(new Vector3(0,0,13f),0.8f).WaitForCompletion();
        }

        if (numberOfShakes == 4)
        {
            yield return battleDialogueBox.SetDialog($"Has capturado un {enemyUnit.Porkemon.Base.Name}!");
            yield return porkeballSpt.DOFade(0, 1f).WaitForCompletion();

            Destroy(porkeballInst);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(.4f);
            porkeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreackOutAnimation();

            if (numberOfShakes <= 2)
                yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} ha escapado!");
            else
                yield return battleDialogueBox.SetDialog("Casi lo has atrapado!");
            Destroy(porkeballInst);

            state = BattleState.LoseTurn;
        }
    }

    int TryToCatchPorkemon(Porkemon porkemon)
    {
        float bonusPorkeball = 1;
        float bonusStat = 1;
        float a = (3 * porkemon.MaxHP / 2 * porkemon.HP) * porkemon.Base.CatchRate * 
            bonusPorkeball * bonusStat / (3 * porkemon.MaxHP);

        if (a >= 255)
            return 4;

        float b = (1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a)));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }
            else
                shakeCount++;
        }
        return shakeCount;
    }
}