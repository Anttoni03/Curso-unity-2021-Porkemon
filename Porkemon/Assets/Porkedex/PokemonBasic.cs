using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Porkemon", menuName = "Porkemon/New porkemon")]
public class PokemonBasic : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;
    [TextArea] [SerializeField] private string description;
    public string Description => description;


    [SerializeField] private int ID;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;



    [SerializeField] private pokemonType type1;
    public pokemonType Type1 => type1;
    [SerializeField] private pokemonType type2;
    public pokemonType Type2 => type2;



    [SerializeField] private int maxHP;
    public int MaxHP => maxHP;
    [SerializeField] private int attack;
    public int Attack => attack;
    [SerializeField] private int defense;
    public int Defense => defense;
    [SerializeField] private int sPAttack;
    public int SPAttack => sPAttack;
    [SerializeField] private int sPDefense;
    public int SPDefense => sPDefense;
    [SerializeField] private int speed;
    public int Speed => speed;


    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => LearnableMoves;
}

public enum pokemonType
{
    Normal,
    Fire,
    Water,
    Grass,
    Electric,
    Ice,
    Fight,
    Poisson,
    Ground,
    Flying,
    Ghost,
    Psychic,
    Rock,
    Steel,
    Dragon,
    Fairy,
    Dark,
    Bug,
    None
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBasic move;
    public MoveBasic Move => move;
    [SerializeField] private int level;
    public int Level => level;
}