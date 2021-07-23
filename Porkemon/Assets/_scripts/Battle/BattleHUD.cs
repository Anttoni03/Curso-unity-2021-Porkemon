using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName;
    [SerializeField] private Text pokemonLevel;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Text pokemonHealth;

    private Porkemon _porkemon;
   

    public void SetPorkemonData(Porkemon porkemon)
    {

        _porkemon = porkemon;

        pokemonName.text = porkemon.Base.Name;
        pokemonLevel.text = $"lv. {porkemon.Level}";

        StartCoroutine(healthBar.SetSmoothHP(porkemon.HP/porkemon.MaxHP));
        pokemonHealth.text = $"{porkemon.HP} / {porkemon.MaxHP}";
        UpdatePokemonData();
    }

    public void UpdatePokemonData()
    {
        StartCoroutine(healthBar.SetSmoothHP((float)_porkemon.HP / _porkemon.MaxHP));
        pokemonHealth.text = $"{_porkemon.HP}/{_porkemon.MaxHP}";
    }
}