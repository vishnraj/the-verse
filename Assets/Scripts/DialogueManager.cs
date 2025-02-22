using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public Text nameText;
    public Text dialogueText;
    public GameObject dialogueBox;
    public GameObject nameBox;
    public string[] dialogueLines;
    public int currentLine;
    public static DialogueManager instance;
    public bool justStarted;

    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    public float delayDuration = 1f; // Time in seconds
    private bool isDialogueEnding = false; // Prevent retriggering during delay

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (dialogueBox.activeInHierarchy && !isDialogueEnding) {
            if (Input.GetButtonUp("Fire1") || Input.GetKeyUp(KeyCode.Space)) {
                if (!justStarted) {
                    ++currentLine;

                    if (currentLine >= dialogueLines.Length) {
                        if (shouldMarkQuest) {
                            shouldMarkQuest = false;
                            if (markQuestComplete) {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            } else {
                                QuestManager.instance.MarkQuestIncomplete(questToMark);
                            }
                        }

                        StartCoroutine(EndDialogueWithDelay());
                    } else {
                        CheckIfName();
                        dialogueText.text = dialogueLines[currentLine];
                    }
                } else {
                    justStarted = false;
                }
            }
        }
    }

    public void ShowDialogue(string[] newLines, bool isPerson) {
        dialogueLines = newLines;
        currentLine = 0;
        CheckIfName();
        dialogueText.text = dialogueLines[currentLine];
        ActivateDialogue();
        nameBox.SetActive(isPerson); 
        justStarted = true;
    }

    public void ActivateDialogue() {
        dialogueBox.SetActive(true);
        GameManager.instance.dialogueActive = true;
    }        

    public void DisableDialogue() {
        dialogueBox.SetActive(false);
        GameManager.instance.dialogueActive = false;
    }

    public void CheckIfName() {
        if (dialogueLines[currentLine].StartsWith("n-")) {
            nameText.text = dialogueLines[currentLine].Split('-')[1];
            ++currentLine;
        }
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete) {
        questToMark = questName;
        markQuestComplete = markComplete;

        shouldMarkQuest = true;
    }

    private IEnumerator EndDialogueWithDelay() {
        isDialogueEnding = true; // Prevent further input
        yield return new WaitForSeconds(delayDuration); // Wait for the delay
        DisableDialogue();
        isDialogueEnding = false; // Re-enable input after delay
    }
}
