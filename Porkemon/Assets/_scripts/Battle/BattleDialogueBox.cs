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

    public float charactersPerSecond;

    public IEnumerator SetDialogue(string message)
    {
        dialogueText.text = "";
        foreach (var character in message)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
    }
}