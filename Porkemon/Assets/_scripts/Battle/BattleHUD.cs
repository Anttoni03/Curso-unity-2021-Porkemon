using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName;
    [SerializeField] private Text pokemonLevel;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Text pokemonHealth;
    [SerializeField] private GameObject expBar;

    private Porkemon _porkemon;


    public void SetPorkemonData(Porkemon porkemon)
    {
        _porkemon = porkemon;

        pokemonName.text = porkemon.Base.Name;
        SetLevelText();
        healthBar.SetHP((float)_porkemon.HP / _porkemon.MaxHP);
        SetXP();
        StartCoroutine(UpdatePokemonData(porkemon.HP));
    }

    public IEnumerator UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthBar.SetSmoothHP((float)_porkemon.HP / _porkemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        yield return null;
    }

    private IEnumerator DecreaseHealthPoints(int oldHpValue)
    {
        while (oldHpValue > _porkemon.HP)
        {
            oldHpValue--;
            pokemonHealth.text = $"{oldHpValue}/{_porkemon.MaxHP}";
            yield return new WaitForSeconds(.1f);
        }
        pokemonHealth.text = $"{_porkemon.HP}/{_porkemon.MaxHP}";
    }

    public void SetXP()
    {
        if (expBar == null)
            return;

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1, 1);
    }

    public IEnumerator SetExperienceSmooth(bool needsToResetBar = false)
    {
        if (expBar == null)
            yield break;

        if (needsToResetBar)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        yield return expBar.transform.DOScaleX(NormalizedExp(), 2f).WaitForCompletion();
    }

    float NormalizedExp()
    {
        float currentLevelExp = _porkemon.Base.GetNecessaryExperienceForLevel(_porkemon.Level);
        float nextLevelExp = _porkemon.Base.GetNecessaryExperienceForLevel(_porkemon.Level + 1);
        float normalizedExp = (_porkemon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokemonLevel.text = $"Lv {_porkemon.Level}";
    }
}
