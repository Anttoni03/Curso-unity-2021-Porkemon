using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBasic _base;
    public int _level;
    public bool isPlayer;

    public Porkemon Porkemon { get; set; }

    public void SetUpPorkemon()
    {
        new Porkemon(_base,_level);

        GetComponent<Image>().sprite = (isPlayer ? Porkemon.Base.BackSprite : Porkemon.Base.FrontSprite);
    }
}
