using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] private Text dialogueText;

    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDescription;

    [SerializeField] private List<Text> movementTexts;
    [SerializeField] private List<Text> actionTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;

    [SerializeField] private Color selectedColor = Color.yellow;

    public float charactersPerSecond = 10F;
    public float timeToWaitAfterText = 1f;

    public bool isWriting = false;

    public IEnumerator SetDialog(string message)
    {
        isWriting = true;
        dialogueText.text = "";
        foreach (var character in message)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        yield return new WaitForSeconds(timeToWaitAfterText);
        isWriting = false;
    }

    public void ToggleDialogText(bool activated)
    {
        dialogueText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDescription.SetActive(activated);
    }

    public void SelectedAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {

            actionTexts[i].color = ((i == selectedAction) ? selectedColor : Color.black);
        }
    }

    public void SetPorkemonsMovements(List<Move> moves)
    {
        Debug.Log(moves.Count);
        for (int i = 0; i < movementTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                movementTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                movementTexts[i].text = "---";
            }
        }
    }

    public void SelectedMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {

            movementTexts[i].color = ((i == selectedMovement) ? selectedColor : Color.black);
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString().ToUpper();

    }
}