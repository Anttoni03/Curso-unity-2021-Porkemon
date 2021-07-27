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
        set => porkemons = value;
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

}
