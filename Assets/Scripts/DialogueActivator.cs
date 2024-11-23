using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour {
    public string[] lines;
    public bool isPerson;

    private bool canActivate;

    public string questToMark;
    public bool markComplete;

    [SerializeField]
    public List<QuestDialogue> questDialogues;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.gameMenuOpen) {
            return;
        }

        if (canActivate && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space)) && !DialogueManager.instance.dialogueBox.activeInHierarchy) {
            string[] dialogueLines = lines; // lines to use as default
            foreach (QuestDialogue quest in questDialogues) {
                // if a quest is marked complete, proceed to the next dialogue for a not complete quest
                if (!QuestManager.instance.CheckIfComplete(quest.questName)) {
                    dialogueLines = quest.lines;
                    break; // always select the first available quest dialogue, for now we're only completing these in order
                }
            }
            DialogueManager.instance.ShowDialogue(dialogueLines, isPerson);
            DialogueManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = false;
        }
    }
}
