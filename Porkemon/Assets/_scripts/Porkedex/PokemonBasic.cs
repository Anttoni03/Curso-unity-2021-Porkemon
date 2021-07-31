using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Porkemon", menuName = "Porkemon/New porkemon")]
public class PokemonBasic : ScriptableObject
{
    [SerializeField] private int ID;


    [SerializeField] private string pName;
    public string Name => pName;


    [TextArea] [SerializeField] private string description;
    public string Description => description;


    [SerializeField] private Sprite frontSprite;
    public Sprite FrontSprite => frontSprite;
    [SerializeField] private Sprite backSprite;
    public Sprite BackSprite => backSprite;


    [SerializeField] private pokemonType type1;
    public pokemonType Type1 => type1;
    [SerializeField] private pokemonType type2;
    public pokemonType Type2 => type2;


    [SerializeField] private int catchRate = 255;
    public int CatchRate => catchRate;


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
    [SerializeField] private int expBase;
    public int ExpBase => expBase;
    [SerializeField] private GrowthRate growthRate;
    public GrowthRate GrowthRate => growthRate;


    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;

    public static int NUMBER_OF_LEARNABLE_MOVES { get; } = 4;

    public int GetNecessaryExperienceForLevel(int level)
    {
        switch (growthRate)
        {
            case GrowthRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 3) / 5);
                break;
            case GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
                break;
            case GrowthRate.MediumSlow:
                return Mathf.FloorToInt(6 * Mathf.Pow(level, 3) / 5 - 15 * Mathf.Pow(level, 2) +
                                        100 * level - 140);
                break;
            case GrowthRate.Slow:
                return Mathf.FloorToInt(5 * Mathf.Pow(level, 3) / 4);
            case GrowthRate.Erratic:
                if (level < 50)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (100 - level) / 50);

                }
                else if (level < 68)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (150 - level) / 100);

                }
                else if (level < 98)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) *
                        Mathf.FloorToInt((1911 - 10 * level) / 3) / 500);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (160 - level) / 100);
                }
                break;
            case GrowthRate.Fluctuating:
                if (level < 15)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt((level + 1) / 3) + 24) / 50);
                }
                else if (level < 36)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (level + 14) / 50);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt(level / 2) + 32) / 50);
                }
                break;
        }

        return -1;
    }
}

public enum GrowthRate
{
    Erratic, Fast, MediumFast,MediumSlow,Slow, Fluctuating
}

public enum pokemonType
{
    None,
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fight,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water
}

public class TypeMatrix
{
    private static float[][] matrix =
    {
    //                   NON   BUG   DAR   DRA   ELE   FAI   FIG   FIR   FLY   GHO   GRA   GRO   ICE   NOR   POI   PSY   ROC   STE   WAT
    /*NON*/ new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f},
    /*BUG*/ new float[] {1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f},
    /*DAR*/ new float[] {1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f},
    /*DRA*/ new float[] {1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f},
    /*ELE*/ new float[] {1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f},
    /*FAI*/ new float[] {1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f},
    /*FIG*/ new float[] {1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 2.0f, 0.5f, 0.5f, 2.0f, 2.0f, 1.0f},
    /*FIR*/ new float[] {1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f},
    /*FLY*/ new float[] {1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f},
    /*GHO*/ new float[] {1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f},
    /*GRA*/ new float[] {1.0f, 0.5f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 0.5f, 2.0f},
    /*GRO*/ new float[] {1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 2.0f, 1.0f},
    /*ICE*/ new float[] {1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f},
    /*NOR*/ new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f},
    /*POI*/ new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f, 0.5f, 0.0f, 1.0f},
    /*PSY*/ new float[] {1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f},
    /*ROC*/ new float[] {1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f},
    /*STE*/ new float[] {1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f},
    /*WAT*/ new float[] {1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f}
    };

    public static float GetMultiplierEffectiveness(pokemonType attackType, pokemonType porkemonDefenderType)
    {
        /*if (attackType == pokemonType.None || porkemonDefenderType == pokemonType.None)
        {
            return 1f;
        }*/

        int row = (int)attackType;
        int col = (int)porkemonDefenderType;

        return matrix[row][col];
    }
}

public class TypeColor
{
    private static Color[] colors =
    {
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

    public static Color GetColorFromType(pokemonType type)
    {
        return colors[(int)type];
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
