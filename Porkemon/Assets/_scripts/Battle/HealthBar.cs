using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    public Color BarColor(float finalScale)
    {
        {
            if (finalScale < 0.15f)
            {
                return new Color(193f / 255, 45f / 255, 45f / 255);
            }
            else if (finalScale < 0.5f)
            {
                return new Color(211f / 255, 211f / 255, 29f / 255);
            }
            else
            {
                return new Color(98f / 255, 178f / 255, 61f / 255);
            }
        }
    }

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normilizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normilizedValue)
    {
        healthBar.transform.localScale = new Vector3(-normilizedValue, 1f);
        healthBar.GetComponent<Image>().color = BarColor(normilizedValue);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(healthBar.GetComponent<Image>().DOColor(BarColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();
    }
}