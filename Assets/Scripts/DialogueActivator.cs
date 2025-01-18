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
        if (GameManager.instance.gameMenuOpen || GameManager.instance.battleActive || GameManager.instance.combatTrialMenuActive) {
            return;
        }

        if (canActivate && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !DialogueManager.instance.dialogueBox.activeInHierarchy) {
            string[] dialogueLines = lines; // lines to use as default

            bool questCheck = false;
            if (questDialogues.Count > 0) {
                QuestDialogue firstNonCompleteQuest = null;
                foreach (var quest in questDialogues) {
                    if (!QuestManager.instance.CheckIfComplete(quest.questName)) {
                        firstNonCompleteQuest = quest;
                        break;
                    }
                }

                if (firstNonCompleteQuest != null) {
                    string[] questMarkerNames = QuestManager.instance.questMarkerNames;
                    for (int i = 0; i < QuestManager.instance.questMarkersComplete.Length; ++i) {
                        if (!QuestManager.instance.questMarkersComplete[i]) {
                            if (questMarkerNames[i] == firstNonCompleteQuest.questName) {
                                dialogueLines = firstNonCompleteQuest.lines;
                            }

                            if (questToMark == questMarkerNames[i]) {
                               questCheck = true;
                            }
                            break;
                        }
                    }
                }
            }

            DialogueManager.instance.ShowDialogue(dialogueLines, isPerson);

            if (questCheck) {
                DialogueManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete);
            }
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
