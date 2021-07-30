using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PorkemonParty : MonoBehaviour
{
    [SerializeField] private List<Porkemon> porkemons;
    public const int NUM_MAX_PORKEMON_IN_PARTY = 6;

    //private List<List<Porkemon>> billBoxes;

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

        /*var box =new List<Porkemon>(13);
        for (int i = 0; i < 6; i++)
        {
            //pcBillBoxes = 
        }*/
    }

    public Porkemon GetFirstNonFaintedPorkemon()
    {
        return porkemons.Where(p => p.HP > 0).FirstOrDefault();
    }

    public int GetPositionFromPorkemon(Porkemon porkemon)
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

    public bool AddPorkemonToParty(Porkemon porkemon)
    {
        if (porkemons.Count < NUM_MAX_PORKEMON_IN_PARTY)
        {
            porkemons.Add(porkemon);
            return true;
        }
        else
        {
            //TODO: Añadir funcionalidad del pc
            return false;
        }
    }
}
