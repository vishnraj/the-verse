using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour {
    public GameObject objectToActivate;

    public int partyMemberIndexToActivate;

    public string questToCheck;

    public bool activeIfComplete;

    private bool initialCheckDone;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (!initialCheckDone) {
            initialCheckDone = true;
            CheckCompletion();
        }
    }

    public void CheckCompletion() {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) {
            if (objectToActivate != null) {
                objectToActivate.SetActive(activeIfComplete);
            } else if (partyMemberIndexToActivate >= 0 && partyMemberIndexToActivate < GameManager.instance.playerStats.Length) {
                GameManager.instance.playerStats[partyMemberIndexToActivate].gameObject.SetActive(activeIfComplete);
            }
        }
    }
}
