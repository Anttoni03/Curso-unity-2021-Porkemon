using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Porkemon
{
    /// <summary>
    /// Referencia a la base de cualquier porkémon
    /// </summary>
    [SerializeField] private PokemonBasic _base;
    /// <summary>
    /// Referencia a la base de cualquier porkémon
    /// </summary>
    public PokemonBasic Base
    {
        get => _base;
    }
    /// <summary>
    /// Nivel actual del porkémon
    /// </summary>
    [SerializeField] private int _level;
    /// <summary>
    /// Nivel actual del porkémon
    /// </summary>
    public int Level
    {
        get => _level;
        set => _level = value;
    }
    /// <summary>
    /// Lista de movimientos del porkémon
    /// </summary>
    private List<Move> _moves;
    /// <summary>
    /// Lista de movimientos del porkémon
    /// </summary>
    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatsBoosted { get; private set; }

    public Queue<string> StatusChangeMessages { get; private set; } = new Queue<string>();

    /// <summary>
    /// Puntos de vida en tiempo real del porkémon
    /// </summary>
    private int _hp;
    /// <summary>
    /// Puntos de vida en tiempo real del porkémon
    /// </summary>
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.FloorToInt(Mathf.Clamp(_hp, 0, MaxHP));
        }
    }

    private int _experience;
    public int Experience
    {
        get => _experience;
        set => _experience = value;
    }

    public Porkemon(PokemonBasic pBase, int plevel)
    {
        _base = pBase;
        _level = plevel;
        InitPorkemon();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="porkemonBase"></param>
    /// <param name="porkemonLevel"></param>
    public void InitPorkemon()
    {
        _experience = Base.GetNecessaryExperienceForLevel(_level);

        _moves = new List<Move>();

        foreach (var movement in _base.LearnableMoves)
        {
            if (movement.Level <= _level)
                _moves.Add(new Move(movement.Move));

            if (_moves.Count >= PokemonBasic.NUMBER_OF_LEARNABLE_MOVES)
                break;
        }

        CalculateStats();
        _hp = MaxHP;

        ResetBostings();
    }

    void ResetBostings()
    {
        StatusChangeMessages = new Queue<string>();
        StatsBoosted = new Dictionary<Stat, int>()
        {
            {Stat.Attack,0 },
            {Stat.Defense,0 },
            {Stat.SPAttack,0 },
            {Stat.SPDefense,0 },
            {Stat.Speed,0 }
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((_base.Attack * _level) / 100f) + 2);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((_base.Defense * _level) / 100f) + 2);
        Stats.Add(Stat.SPAttack, Mathf.FloorToInt((_base.SPAttack * _level) / 100f) + 2);
        Stats.Add(Stat.SPDefense, Mathf.FloorToInt((_base.SPDefense * _level) / 100f) + 2);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((_base.Speed * _level) / 100f) + 2);

        MaxHP = Mathf.FloorToInt((_base.MaxHP * _level) / 20f) + 10;
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boost = StatsBoosted[stat];
        float multiplier = 1f + Mathf.Abs(boost) / 2f;

        if (boost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * multiplier);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / multiplier);
        }
        return statValue;
    }

    public void ApplyBoost(StatBoosting boost)
    {
        var stat = boost.stat;
        var value = boost.boost;

        StatsBoosted[stat] = Mathf.Clamp(StatsBoosted[stat] + value, -6, 6);
        if (value > 0)
        {
            StatusChangeMessages.Enqueue($"{Base.Name} ha incrementado su {stat}");
        }
        else
        {
            StatusChangeMessages.Enqueue($"{Base.Name} ha reducido su {stat}");
        }
        //Debug.Log($"{stat} se ha modificado a {StatsBoosted[stat]}");
    }

    /// <summary>
    /// Puntos de vida máximos del porkémon
    /// </summary>
    public int MaxHP { get; private set; }
    public int Attack => GetStat(Stat.Attack);
    public int Defense => GetStat(Stat.Defense);
    public int SPAttack => GetStat(Stat.SPAttack);
    public int SPDefense => GetStat(Stat.SPDefense);
    public int Speed => GetStat(Stat.Speed);


    public DamageDescription ReceiveDamage(Porkemon attacker, Move move)
    {
        float critical = 1f;
        if (Random.Range(0, 100f) < 8f)
            critical = 2f;

        float type1 = TypeMatrix.GetMultiplierEffectiveness(move.Base.Type, this.Base.Type1);
        float type2 = TypeMatrix.GetMultiplierEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDesc = new DamageDescription()
        {
            Critical = critical,
            Type = type1 * type2,
            Fainted = false
        };

        float attack = (move.Base.IsSpecialMove ? attacker.SPAttack : attacker.Attack);
        float defense = (move.Base.IsSpecialMove ? this.SPDefense : this.Attack);

        float modifiers = Random.Range(0.85f, 1f) * type1 * type2 * critical;
        float baseDamage = (2 * attacker.Level / 5f + 2) * move.Base.Power 
            * (attack / (float)defense) / 50f + 2;
        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        #region Cosa mía(sin daño si movimiento de estado)
        if (move.Base.Category == MovementCategory.Status)
            totalDamage = 0;
        #endregion

        HP -= totalDamage;
        if (HP <= 0)
        {
            HP = 0;
            damageDesc.Fainted = true;
        }

        return damageDesc;
    }

    public Move RandomMove()
    {
        var movesWithPP = Moves.Where(m => m.PP > 0).ToList();
        if (movesWithPP.Count > 0)
        {
            int randId = Random.Range(0, movesWithPP.Count);
            return movesWithPP[randId];
        }

        return null;
    }

    public bool NeedsToLevelUp()
    {
        if (Experience > Base.GetNecessaryExperienceForLevel(_level + 1))
        {
            int currentMaxHP = MaxHP;
            _level++;
            HP += (MaxHP - currentMaxHP);
            return true;
        }
        else
        {
            return false;
        }
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove learlableMove)
    {
        if (Moves.Count >= PokemonBasic.NUMBER_OF_LEARNABLE_MOVES)
        {
            return;
        }

        Moves.Add(new Move(learlableMove.Move));
    }

    public void OnBattleFinish()
    {
        ResetBostings();
    }

}



public class DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}
