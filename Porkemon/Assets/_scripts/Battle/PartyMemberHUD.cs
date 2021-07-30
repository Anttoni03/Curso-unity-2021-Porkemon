using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Text nameText, levelText, typeText, hpText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Image porkemonImage;

    private Porkemon _porkemon;

    [SerializeField] private Color selectedColor = new Color(.2f, .4f, .9f);

    public void SetPorkemonData(Porkemon porkemon)
    {
        _porkemon = porkemon;

        nameText.text = porkemon.Base.Name;
        levelText.text = $"lv. {porkemon.Level}";

        //Si hay que poner el tipo
        /*if (porkemon.Base.Type2 == pokemonType.None)
        {
            typeText.text = porkemon.Base.Type1.ToString().ToUpper();
        }
        else
        {
            typeText.text = $"{porkemon.Base.Type1.ToString().ToUpper()} - {porkemon.Base.Type2.ToString().ToUpper()}";
        }*/

        hpText.text = $"{porkemon.HP}/{porkemon.MaxHP}";
        healthBar.SetHP((float)porkemon.HP/porkemon.MaxHP);
        porkemonImage.sprite = porkemon.Base.FrontSprite;

        GetComponent<Image>().color = TypeColor.GetColorFromType(porkemon.Base.Type1);
    }

    public void SetSelectedPorkemon(bool selected)
    {
        if (selected)
        {
            nameText.color = selectedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}