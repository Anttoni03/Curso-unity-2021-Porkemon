using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porkemon
{
    private PokemonBasic _base;
    private int _level;

    public Porkemon(PokemonBasic porkemonBase, int porkemonLevel)
    {
        _base = porkemonBase;
        _level = porkemonLevel;
    }

    public int MaxHP => Mathf.FloorToInt((_base.MaxHP * _level) / 100f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100f) + 2;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100f) + 2;
    public int SPAttack => Mathf.FloorToInt((_base.SPAttack * _level) / 100f) + 2;
    public int SPDefense => Mathf.FloorToInt((_base.SPDefense * _level) / 100f) + 2;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100f) + 2;
}
