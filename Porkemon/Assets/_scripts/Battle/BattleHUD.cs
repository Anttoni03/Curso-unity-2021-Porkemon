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
        
        healthbar.SetHP((float) _porkemon.HP / _porkemon.MaxHP);
        UpdatePokemonData(porkemon.HP);
        
        //Quitar si innecesario
        /*StartCoroutine(healthBar.SetSmoothHP(porkemon.HP/porkemon.MaxHP));
        pokemonHealth.text = $"{porkemon.HP} / {porkemon.MaxHP}";
        UpdatePokemonData(porkemon.HP);*/
    }

    public void UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthBar.SetSmoothHP((float)_porkemon.HP / _porkemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        
        //Quitar si innecesario
        //pokemonHealth.text = $"{_porkemon.HP}/{_porkemon.MaxHP}";
    }

    public IEnumerator DecreaseHealthPoints(int oldHpValue)
    {
        while (oldHpValue > _porkemon.HP)
        {
            oldHpValue--;
            pokemonHealth.text = $"{oldHpValue}/{_porkemon.MaxHP}";
            yield return new WaitForSeconds(.1f);
        }
        pokemonHealth.text = $"{_porkemon.HP}/{_porkemon.MaxHP}";
    }
}
