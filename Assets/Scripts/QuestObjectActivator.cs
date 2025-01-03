using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour {
    public GameObject objectToActivate;

    public int partyMemberIndexToActivate;

    public string questToCheck;

    public bool activeIfComplete;

    public bool deactivateActivatorObjectAfter;

    private bool initialCheckDone;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // prevents any behavior that is tied to this
        // if we intended to destroy this after the quest was completed
        // the first time, but scene got reloaded
        if (deactivateActivatorObjectAfter) {
            if (QuestManager.instance.CheckIfComplete(questToCheck)) {
                gameObject.SetActive(false);
                return;
            }
        }

        if (!initialCheckDone) {
            initialCheckDone = true;
            CheckCompletion();
        }
    }

    public void CheckCompletion() {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) {
            if (objectToActivate != null) {
                objectToActivate.SetActive(activeIfComplete);

                if (deactivateActivatorObjectAfter) {
                    gameObject.SetActive(false);
                }
            } else if (partyMemberIndexToActivate >= 0 && partyMemberIndexToActivate < GameManager.instance.playerStats.Length) {
                GameManager.instance.playerStats[partyMemberIndexToActivate].gameObject.SetActive(activeIfComplete);

                if (deactivateActivatorObjectAfter) {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
