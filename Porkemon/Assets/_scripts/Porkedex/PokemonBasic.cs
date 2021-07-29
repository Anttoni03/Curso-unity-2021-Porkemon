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

    [SerializeField] private int catchRate = 255;
    public int CatchRate => catchRate;

    [SerializeField] private int expBase;
    public int ExpBase => expBase;
    [SerializeField] private GrowthRate growthRate;
    public GrowthRate GrowthRate => growthRate;


    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;



    public int GetNecessaryExperienceForLevel(int level)
    {
        switch (growthRate)
        {
            case global::GrowthRate.Erratic:
                if (level < 50)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (100 - level) / 50);
                }
                else if (level < 78)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (150 - level) / 50);
                }
                else if (level < 98)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * Mathf.FloorToInt((1911 - 10*level) / 3)/500);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (160 - level) / 100);
                }
                break;
            case global::GrowthRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 3) / 5);
                break;
            case global::GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
                break;
            case global::GrowthRate.MediumSlow:
                return Mathf.FloorToInt(6*Mathf.Pow(level, 3)/5 - 15*Mathf.Pow(level,2) + 100*level - 140);
                break;
            case global::GrowthRate.Slow:
                //TODO: Arreglar
                return -1;
                break;
            case global::GrowthRate.Fluctuating:
                if (level < 15)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt(level + 1 / 3) + 2));
                }
                else if (level < 36)
                {
                    //TODO: Arreglar
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt(level + 1 / 3) + 2));
                }
                else
                {
                    //TODO: Arreglar
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt(level + 1 / 3) + 2));
                }
                break;
            default:
                return -1;
                break;
        }
    }
}

public enum GrowthRate
{
    Erratic, Fast, MediumFast,MediumSlow,Slow, Fluctuating
}

public enum pokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fight,
    Poisson,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
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

/*public class TypeColor
{
    private static Color[] colors = new
        Color.white, // None
        new Color(0.8193042f, 0.9333333f, 0.5254902f), // Bug
        new Color(0.735849f, 0.6178355f, 0.5588287f), // Dark
        new Color(0.6556701f, 0.5568628f, 0.7647059f), // Dragon
        new Color(0.9942768f, 1f, 0.5707547f), // Electric
        new Color(0.9339623f, 0.7621484f, 0.9339623f), // Fairy
        new Color(0.735849f, 0.5600574f, 0.5310609f), // Fight
        new Color(0.990566f, 0.5957404f, 0.5279903f), // Fire
        new Color(0.7358491f, 0.7708895f, 0.9811321f), // Flying
        new Color(0.6094251f, 0.6094251f, 0.7830189f), // Ghost
        new Color(0.4103774f, 1, 0.6846618f), // Grass
        new Color(0.9433962f, 0.7780005f, 0.5562478f), // Ground
        new Color(0.7216981f, 0.9072328f, 1), // Ice
        new Color(0.8734059f, 0.8773585f, 0.8235582f), // Normal
        new Color(0.6981132f, 0.4774831f, 0.6539872f), // Poison
        new Color(1, 0.6650944f, 0.7974522f), // Psychic
        new Color(0.8584906f, 0.8171859f, 0.6519669f), // Rock
        new Color(0.7889819f, 0.7889819f, 0.8490566f), // Steel
        new Color(0.5613208f, 0.7828107f, 1) // Water
    };



public Color GetColorFromType(pokemonType type)
    {
        return colors[(int)type];
    }
}*/


[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBasic move;
    public MoveBasic Move => move;
    [SerializeField] private int level;
    public int Level => level;
}
