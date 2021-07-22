using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    /*private void Start()
    {
        SetHP(.5f);
    }*/

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normilizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHP(float normilizedValue)
    {
        healthBar.transform.localScale = new Vector3(-normilizedValue, 1, 1);
    }

}
