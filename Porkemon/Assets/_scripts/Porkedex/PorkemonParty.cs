using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PorkemonParty : MonoBehaviour
{
    [SerializeField] private List<Porkemon> porkemons;
    public List<Porkemon> Porkemons
    {
        get => porkemons;
    }

    private void Start()
    {
        foreach (var porkemon in porkemons)
        {
            porkemon.InitPorkemon();
        }
    }

    public Porkemon GetFirstNonFaintedPorkemon()
    {
        return porkemons.Where(p => p.HP > 0).FirstOrDefault();
    }

    public int GetPositionFirstNonFaintedPorkemon(Porkemon porkemon)
    {
        for (int i = 0; i < Porkemons.Count; i++)
        {
            if (porkemon == Porkemons[i])
            {
                return i;
            }
        }
        return -1;
    }

}
