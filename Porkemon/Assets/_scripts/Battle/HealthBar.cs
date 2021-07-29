using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    public Color barColor(float finalScale)
    {
        {
            if (finalScale < 0.15f)
            {
                return new Color((float)193 / 255, 45 / 255, 45 / 255);
            }
            else if (finalScale < 0.5f)
            {
                return new Color((float)211 / 255, 212 / 255, 29 / 255);
            }
            else
            {
                return new Color((float)98 / 255, (float)178 / 255, (float)61 / 255);
            }
        }
    }

    private Image _barImage;

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normilizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normilizedValue)
    {
        healthBar.transform.localScale = new Vector3(-normilizedValue, 1f);
        healthBar.GetComponent<Image>().color = barColor(normilizedValue);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        /*
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            healthBar.GetComponent<Image>().color = barColor;
            yield return null;
        }
        healthBar.transform.localScale = new Vector3(normalizedValue, 1);*/

        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(healthBar.GetComponent<Image>().DOColor(barColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();

    }
}