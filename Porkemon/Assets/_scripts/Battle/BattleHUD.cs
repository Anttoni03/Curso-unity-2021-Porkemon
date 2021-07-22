using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName;
    [SerializeField] private Text pokemonLevel;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Text healthText;
   

    public void SetPorkemonData(Porkemon porkemon)
    {
        pokemonName.text = porkemon.Base.Name;
        pokemonLevel.text = $"lv. {porkemon.Level}";

        healthBar.SetHP(porkemon.HP/porkemon.MaxHP);
        healthText.text = $"{porkemon.HP} / {porkemon.MaxHP}";
    }
}
