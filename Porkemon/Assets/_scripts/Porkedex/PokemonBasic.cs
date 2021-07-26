using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Porkemon", menuName = "Porkemon/New porkemon")]
public class PokemonBasic : ScriptableObject
{
    [SerializeField] private string pName;
    public string Name => pName;
    [TextArea] [SerializeField] private string description;
    public string Description => description;


    [SerializeField] private int ID;

    [SerializeField] private Sprite frontSprite;
    public Sprite FrontSprite => frontSprite;
    [SerializeField] private Sprite backSprite;
    public Sprite BackSprite => backSprite;



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
    public List<LearnableMove> LearnableMoves => learnableMoves;
}

public enum pokemonType
{
    None,
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
    Bug
}

public class TypeMatrix
{
    static float[][] matrix =
    {
        //               NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
    /*NOR*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , .5F, 1F , 1F , 1F , .5F, 1F },
    /*FIR*/new float [] { 1F , .5F, .5F, 1F , 2F , 2F , 1F , 1F , 1F , 1F , 1F , 2F , .5F, 1F , .5F, 1F , 2F , 1F },
    /*WAT*/new float [] { 1F , 2F , .5F, 1F , .5F, 1F , 1F , 1F , 2F , 1F , 1F , 1F , 2F , 1F , .5F, 1F , 1F , 1F },
    /*ELE*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*GRA*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*ICE*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*FIG*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*POI*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*GRO*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*FLY*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*PSY*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*BUG*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*ROC*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*GHO*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*DRA*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*DAR*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*STE*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F },
    /*FAI*/new float [] { 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F , 1F }
    };
    //TODO: Tabla de tipos

    public static float GetMultiplierEffectiveness(pokemonType attackType, pokemonType porkemonDefenderType)
    {
        if (attackType == pokemonType.None || porkemonDefenderType == pokemonType.None)
        {
            return 1f;
        }
        int row = (int)attackType;
        int col = (int)porkemonDefenderType;

        return matrix[row - 1][col - 1];
    }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBasic move;
    public MoveBasic Move => move;
    [SerializeField] private int level;
    public int Level => level;
}