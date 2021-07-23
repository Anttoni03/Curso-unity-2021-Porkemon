using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normilizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normilizedValue)
    {
        healthBar.transform.localScale = new Vector3(-normilizedValue, 1, 1);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1);
            yield return null;
        }
        healthBar.transform.localScale = new Vector3(normalizedValue, 1);
    }

}
