using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorkemonMapArea : MonoBehaviour
{
    [SerializeField] private List<Porkemon> wildPorkemons;

    public Porkemon GetRandomWildPorkemon()
    {
        var porkemon = wildPorkemons[Random.Range(0,wildPorkemons.Count)];
        porkemon.InitPorkemon();
        return porkemon;
    }
}
