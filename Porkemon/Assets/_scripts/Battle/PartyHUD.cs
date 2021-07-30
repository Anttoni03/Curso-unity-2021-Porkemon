using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] private Text messageText;
    private PartyMemberHUD[] memberHuds;

    private List<Porkemon> porkemons;

    public void InitPartyHUD()
    {
        memberHuds = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void SetPartyData(List<Porkemon> porkemons)
    {

        this.porkemons = porkemons;
        
        if (messageText != null)
            messageText.text = "Selecciona un porkémon";

        for (int i = 0; i < memberHuds.Length; i++)
        {
            if (i < porkemons.Count)
            {
                memberHuds[i].SetPorkemonData(porkemons[i]);
                memberHuds[i].gameObject.SetActive(true);
            }
            else
                memberHuds[i].gameObject.SetActive(false);
        }
    }

    public void UpdateSelectedPorkemon(int selectedPorkemon)
    {
        for (int i = 0; i < porkemons.Count; i++)
        {
            memberHuds[i].SetSelectedPorkemon(i == selectedPorkemon);
        }
    }

    public void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }
}
