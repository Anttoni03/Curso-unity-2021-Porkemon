using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Linq;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemsSelectScreen,
    LoseTurn,
    FinishBattle,
    ForgetMovement
}

public enum BattleType
{
    WildPorkemon,
    Trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogueBox battleDialogueBox;

    [SerializeField] private PartyHUD partyHUD;

    [SerializeField] private SelectionMovementUI selectionMovementUI;

    [SerializeField] private GameObject porkeball;

    public BattleState state;

    public BattleType type;

    public event Action<bool> OnBattleFinish;

    private PorkemonParty playerParty;
    private Porkemon wildPorkemon;

    private float timeSinceLastClick;
    [SerializeField] private float TimeBetweenClicks = .15f;

    private int currentSelectedAction;
    private int currentSelectedMovement;
    private int currentSelectedPorkemon;

    private int escapeAttempts;
    private MoveBasic moveToLearn;

    public AudioClip attackClip, damageClip, levelUpClip, endBattleClip, porkeballClip;

    public void HandleStartBattle(PorkemonParty playerParty, Porkemon wildPorkemon)
    {
        type = BattleType.WildPorkemon;
        escapeAttempts = 0;
        this.playerParty = playerParty;
        this.wildPorkemon = wildPorkemon;
        StartCoroutine(SetupBattle());
    }

    public void HandleStartTrainerBattle(PorkemonParty porkemonParty, PorkemonParty trainerParty, bool isLeader)
    {
        type = (isLeader ? BattleType.Leader : BattleType.Trainer);
        //TODO: Adaptar batalla para entrenador
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
            battleDialogueBox.ToggleActions(false);
            battleDialogueBox.ToggleMovements(false);
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
        SoundManager.SharedInstance.PlaySound(endBattleClip);
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
        currentSelectedPorkemon = playerParty.GetPositionFromPorkemon(playerUnit.Porkemon);
        partyHUD.UpdateSelectedPorkemon(currentSelectedPorkemon);
    }

    void OpenInventoryScreen()
    {
        //TODO: Inventario e ítems
        print("Abrir inventario");
        battleDialogueBox.ToggleDialogText(false);
        StartCoroutine(ThrowPorkeball());
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if ((timeSinceLastClick < TimeBetweenClicks) || battleDialogueBox.isWriting)
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
        else if (state == BattleState.ForgetMovement)
        {
            selectionMovementUI.HandleForgetMoveSelection((moveIndex) => 
            {
                if (moveIndex < 0)
                {
                    timeSinceLastClick = 0;
                    return;
                }

                StartCoroutine(ForgetOldMove(moveIndex));
            });
        }
    }

    IEnumerator ForgetOldMove(int moveIndex)
    {
        selectionMovementUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBasic.NUMBER_OF_LEARNABLE_MOVES)
        {
            StartCoroutine(battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} no ha aprendido {moveToLearn.Name}"));
        }
        else
        {
            //Olvidación del movimiento seleccionado en pos del nuevo
            var selectedMove = playerUnit.Porkemon.Moves[moveIndex].Base;
            yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} ha olvidado {selectedMove.Name} aprendió {moveToLearn.Name}");
            playerUnit.Porkemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        //TODO: Revisar para cuando haya entrenadores
        state = BattleState.FinishBattle;
    }

    private void HandlePlayerActionSelection()
    {
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
                //Mochila
                OpenInventoryScreen();
            }
            else if (currentSelectedAction == 3)
            {
                //Huida
                StartCoroutine(TryToEscapeFromBattle());
            }
        }
    }

    void HandlePlayerMovementSelection()
    {
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
            currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2 * Mathf.FloorToInt(currentSelectedMovement / 2);
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
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPorkemon -= (int)Input.GetAxisRaw("Vertical") * 2;
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
            partyHUD.gameObject.SetActive(false);
            state = BattleState.Busy;
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
            PlayerMovementSelection();
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
        SoundManager.SharedInstance.PlayMusic(attackClip);
        yield return new WaitForSeconds(1f);
        SoundManager.SharedInstance.PlayMusic(damageClip);
        target.PlayReceiveAttackAnimation();

        var damageDesc = target.Porkemon.ReceiveDamage(attacker.Porkemon, move);
        yield return target.Hud.UpdatePokemonData(oldHpValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return HandlePorkemonFainted(target);
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

        //Borrar si innecesario
        /*partyHUD.gameObject.SetActive(false);
        state = BattleState.Busy;*/

        StartCoroutine(PerformEnemyMovement());
    }

    IEnumerator ThrowPorkeball()
    {
        state = BattleState.Busy;

        if (type != BattleType.WildPorkemon)
        {
            yield return battleDialogueBox.SetDialog("No puedes robar porkémons de otros entrenadores");
            state = BattleState.LoseTurn;
            yield break;
        }


        yield return battleDialogueBox.SetDialog($"Has lanzado una {(porkeball.name).ToLower()}");

        var porkeballInst = Instantiate(porkeball, playerUnit.transform.position - 
            new Vector3(2,0), Quaternion.identity);

        var porkeballSpt = porkeballInst.GetComponent<SpriteRenderer>();

        yield return porkeballSpt.transform.DOLocalJump(enemyUnit.transform.position + 
            new Vector3(0,2),2f,1,1f).WaitForCompletion();

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

            if (playerParty.AddPorkemonToParty(enemyUnit.Porkemon))
                yield return battleDialogueBox.SetDialog($"{enemyUnit.Porkemon.Base.Name} se ha añadido a tu equipo!");
            else
                yield return battleDialogueBox.SetDialog("En algún momento se mandará al pc. Espere por favor");

            Destroy(porkeballInst);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(.4f);
            porkeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

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
            if (Random.Range(0, 65535) >= b)
                break;
            else
                shakeCount++;
        }
        return shakeCount;
    }

    IEnumerator TryToEscapeFromBattle()
    {
        state = BattleState.Busy;

        if (type != BattleType.WildPorkemon)
        {
            yield return battleDialogueBox.SetDialog("No puedes huir de combates contra entrenadores");
            state = BattleState.LoseTurn;
            yield break;
        }

        escapeAttempts++;

        int playerSpeed = playerUnit.Porkemon.Speed;
        int enemySPeed = enemyUnit.Porkemon.Speed;

        if (playerSpeed >= enemySPeed)
        {
            yield return battleDialogueBox.SetDialog("Has huido con éxito.");
            yield return new WaitForSeconds(0.8f);
            OnBattleFinish(true);
        }
        else
        {
            int oddsEscape = (Mathf.FloorToInt(playerSpeed * 128 / enemySPeed) + 30 * escapeAttempts) % 256;
            if (Random.Range(0,256) < oddsEscape)
            {
                yield return battleDialogueBox.SetDialog("Has huido con éxito.");
                yield return new WaitForSeconds(0.8f);
                OnBattleFinish(true);
            }
            else
            {
                yield return battleDialogueBox.SetDialog("No has podido escapar!");
                state = BattleState.LoseTurn;
            }
        }
    }

    IEnumerator HandlePorkemonFainted(BattleUnit faintedUnit)
    {
        yield return battleDialogueBox.SetDialog($"{faintedUnit.Porkemon.Base.Name} se ha debilitado.");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayer)
        {
            //TODO: Gain exp + check new level
            int expBase = faintedUnit.Porkemon.Base.ExpBase;
            int level = faintedUnit.Porkemon.Level;
            float multiplier = (type == BattleType.WildPorkemon ? 1f : 1.5f);
            int wonExp = Mathf.FloorToInt(expBase * level * multiplier / 7);
            playerUnit.Porkemon.Experience += wonExp;
            yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.name} ha ganado {wonExp} puntos de experiencia.");
            yield return playerUnit.Hud.SetExperienceSmooth();
            yield return new WaitForSeconds(1f);

            //TODO: Check level
            while (playerUnit.Porkemon.NeedsToLevelUp())
            {
                SoundManager.SharedInstance.PlaySound(levelUpClip);
                playerUnit.Hud.SetLevelText();
                yield return playerUnit.Hud.UpdatePokemonData(playerUnit.Porkemon.HP);
                yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} ha subido de nivel");

                //TODO: Intentar aprender nuevo movimiento

                var newLearneableMove = playerUnit.Porkemon.GetLearnableMoveAtCurrentLevel();
                if (newLearneableMove != null)
                {
                    if (playerUnit.Porkemon.Moves.Count < PokemonBasic.NUMBER_OF_LEARNABLE_MOVES)
                    {
                        playerUnit.Porkemon.LearnMove(newLearneableMove);
                        yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} ha aprendido {newLearneableMove.Move.Name}");
                        battleDialogueBox.SetPorkemonsMovements(playerUnit.Porkemon.Moves);
                    }
                    else
                    {
                        yield return battleDialogueBox.SetDialog($"{playerUnit.Porkemon.Base.Name} quiere aprender {newLearneableMove.Move.Name}");
                        yield return battleDialogueBox.SetDialog($"Pero no puede aprender más de 4 movimientos");
                        yield return ChooseMovementToForget(playerUnit.Porkemon, newLearneableMove.Move);
                        yield return new WaitUntil(() => state != BattleState.ForgetMovement);
                    }
                }
                yield return playerUnit.Hud.SetExperienceSmooth(true);
            }
        }
        CheckForBattleFinish(faintedUnit);
    }

    IEnumerator ChooseMovementToForget(Porkemon learner, MoveBasic newMove)
    {
        //TODO: Asegurar que es busy
        state = BattleState.Busy;
        yield return battleDialogueBox.SetDialog($"Selecciona el movimiento que {learner.Base.Name} va a olvidar");
        selectionMovementUI.gameObject.SetActive(true);
        selectionMovementUI.SetMovements(learner.Moves.Select(mv => mv.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState.ForgetMovement;
    }
}