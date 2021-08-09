using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private GameObject[] porkemonAreas;

    private GameState _gameState;

    public AudioClip worldClip, battleClip;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    private void Start()
    {
        SoundManager.SharedInstance.PlayMusic(worldClip);
        playerController.OnPorkemonEncounter += StartPorkemonBattle;
        battleManager.OnBattleFinish += FinishPorkemonBattle;
    }

    public void StartPorkemonBattle()
    {
        SoundManager.SharedInstance.PlayMusic(battleClip);

        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PorkemonParty>();

        //Línea modificada para elegir la hierba y porkémons salvajes
        var wildPorkemon = playerController.CheckNearestPorkemonArea(porkemonAreas).GetComponent<PorkemonMapArea>().GetRandomWildPorkemon();
        
        var wildPorkemonCopy = new Porkemon(wildPorkemon.Base, wildPorkemon.Level);

        battleManager.HandleStartBattle(playerParty, wildPorkemonCopy);
    }

    public void FinishPorkemonBattle(bool playerHasWon)
    {
        SoundManager.SharedInstance.PlayMusic(worldClip);

        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
        if (!playerHasWon)
        {
            //TODO: Victoria y derrota
        }
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
}