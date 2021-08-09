using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normilizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normilizedValue)
    {
        healthBar.transform.localScale = new Vector3(normilizedValue, 1f);
        healthBar.GetComponent<Image>().color = ColorManager.SharedInstance.BarColor(normilizedValue);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(healthBar.GetComponent<Image>().DOColor(ColorManager.SharedInstance.BarColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();
    }
}