using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Porkemon/New movement")]
public class MoveBasic : ScriptableObject
{
    [SerializeField] private string mName;
    public string Name => mName;
    [TextArea] [SerializeField] private string description;
    public string Description => description;



    [SerializeField] private pokemonType type;
    public pokemonType Type => type;
    [SerializeField] private MovementCategory category;
    public MovementCategory Category => category;
    [SerializeField] private int power;
    public int Power => power;
    [SerializeField] private int accuracy;
    public int Accuracy => accuracy;
    [SerializeField] private int pp;
    public int PP => pp;

    public enum MovementCategory
    {
        Physical,
        Special,
        Status
    }

    public bool IsSpecialMove
    {
        get
        {
            if (category == MovementCategory.Special)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
