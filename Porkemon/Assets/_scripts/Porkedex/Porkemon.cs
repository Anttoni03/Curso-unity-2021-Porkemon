using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porkemon
{
    /// <summary>
    /// Referencia a la base de cualquier porkémon
    /// </summary>
    private PokemonBasic _base;
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
    private int _level;
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
    /// Puntos de vida en tiempo real del porkémon
    /// </summary>
    private int _hp;
    /// <summary>
    /// Lista de movimientos del porkémon
    /// </summary>
    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }
    /// <summary>
    /// Puntos de vida en tiempo real del porkémon
    /// </summary>
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="porkemonBase"></param>
    /// <param name="porkemonLevel"></param>
    public Porkemon(PokemonBasic porkemonBase, int porkemonLevel)
    {
        _base = porkemonBase;
        _level = porkemonLevel;

        _hp = MaxHP;

        _moves = new List<Move>();

        foreach (var movement in _base.LearnableMoves)
        {
            if (movement.Level <= _level)
                _moves.Add(new Move(movement.Move));

            if (_moves.Count > 4)
                break;
        }
    }

    /// <summary>
    /// Puntos de vida máximos del porkémon
    /// </summary>
    public int MaxHP => Mathf.FloorToInt((_base.MaxHP * _level) / 20f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100f) + 2;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100f) + 2;
    public int SPAttack => Mathf.FloorToInt((_base.SPAttack * _level) / 100f) + 2;
    public int SPDefense => Mathf.FloorToInt((_base.SPDefense * _level) / 100f) + 2;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100f) + 2;


    public bool ReceiveDamage(Porkemon attacker, Move move)
    {
        //TODO: Acabar fórmula de daño
        float modifiers = Random.Range(0.85f, 1f);
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power 
            * (attacker.Attack / (float)Defense)) / 50f + 2;

        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        HP -= totalDamage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public Move RandomMove()
    {
        int randID = Random.Range(0, Moves.Count);
        return Moves[randID];
    }

}