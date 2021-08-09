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

    [SerializeField] private MoveStatEffect effects;
    public MoveStatEffect Effects => effects;
    [SerializeField] private MoveTarget target;
    public MoveTarget Target => target;



    public bool IsSpecialMove => category == MovementCategory.Special;

}

public enum MovementCategory
{
    Physical,
    Special,
    Status
}

[System.Serializable]
public class MoveStatEffect
{
    [SerializeField] private List<StatBoosting> boostings;
    public List<StatBoosting> Boostings => boostings;
}

[System.Serializable]
public class StatBoosting
{
    public Stat stat;
    public int boost;
    public MoveTarget target;
}

public enum MoveTarget
{
    Self, Other
}