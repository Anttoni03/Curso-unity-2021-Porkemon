using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Travel,
    Battle
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;

    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    private void Start()
    {
        playerController.OnPorkemonEncounter += StartPorkemonBattle;
        battleManager.OnBattleFinish += FinishPorkemonBattle;
    }

    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
            playerController.HandleUpdate();
        }
        else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }

    public void StartPorkemonBattle()
    {
        _gameState = GameState.Battle;

        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        battleManager.HandleStartBattle();
    }

    public void FinishPorkemonBattle(bool playerHasWon)
    {
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
    }
}
