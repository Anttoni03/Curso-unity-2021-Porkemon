using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMovementUI : MonoBehaviour
{
    [SerializeField] private Text[] movementTexts;
    [SerializeField] private Color selectedColor;
    private int currentSelectedMovement = 0;

    public void SetMovements(List<MoveBasic> porkemonMoves, MoveBasic newMove)
    {
        currentSelectedMovement = 0;

        for (int i = 0; i < porkemonMoves.Count; i++)
        {
            movementTexts[i].text = porkemonMoves[i].Name;
        }

        movementTexts[porkemonMoves.Count].text = newMove.Name;
    }

    public void HandleForgetMoveSelection(Action<int> onSelected)
    {

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            int direction = Mathf.FloorToInt(Input.GetAxisRaw("Vertical"));
            currentSelectedMovement -= direction;
            onSelected?.Invoke(-1);
        }

            currentSelectedMovement = Mathf.Clamp(currentSelectedMovement, 0, PokemonBasic.NUMBER_OF_LEARNABLE_MOVES);
            UpdateColorForgetMoveSelection(currentSelectedMovement);

        if (Input.GetAxisRaw("Submit") != 0)
        {
            onSelected?.Invoke(currentSelectedMovement);
        }
    }

    public void UpdateColorForgetMoveSelection(int selectedMove)
    {
        for (int i = 0; i <= PokemonBasic.NUMBER_OF_LEARNABLE_MOVES; i++)
        {
            movementTexts[i].color = (i == selectedMove ? selectedColor : Color.black);
        }
    }
}
