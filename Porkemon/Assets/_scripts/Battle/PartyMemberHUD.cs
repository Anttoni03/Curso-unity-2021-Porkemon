using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Text nameText, levelText, typeText, hpText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Image porkemonImage;
    [SerializeField] private Color color = new Color(.2f, .4f, .9f);

    private Porkemon _porkemon;

    public void SetPorkemonData(Porkemon porkemon)
    {
        _porkemon = porkemon;

        nameText.text = porkemon.Base.Name;
        levelText.text = $"lv. {porkemon.Level}";
        //typeText.text = porkemon.Base.Type1.ToString();       Si hay que poner el tipo
        hpText.text = $"{porkemon.HP}/{porkemon.MaxHP}";
        healthBar.SetHP((float)porkemon.HP/porkemon.MaxHP);
        porkemonImage.sprite = porkemon.Base.FrontSprite;
    }

    public void SetSelectedPorkemon(bool selected)
    {
        if (selected)
        {
            nameText.color = color;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
