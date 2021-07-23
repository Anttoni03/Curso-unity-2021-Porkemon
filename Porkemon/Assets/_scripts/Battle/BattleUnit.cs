using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBasic _base;
    public int _level;
    public bool isPlayer;

    public Porkemon Porkemon { get; set; }

    private Image porkemonImage;
    private Vector3 initialPosition;

    private void Awake()
    {
        porkemonImage = GetComponent<Image>();
        initialPosition = porkemonImage.transform.localPosition;
    }


    public void SetUpPorkemon()
    {
        Porkemon = new Porkemon(_base,_level);

        porkemonImage.sprite = (isPlayer ? Porkemon.Base.BackSprite : Porkemon.Base.FrontSprite);
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {

        porkemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 400,
            initialPosition.y);

        porkemonImage.transform.DOLocalMoveX(initialPosition.x, .8f);
    }

    public void PlayAttackAnimation()
    {

    }

    public void PlayFaintAnimation()
    {
        
    }
}