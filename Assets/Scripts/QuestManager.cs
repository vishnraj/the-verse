using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            if (instance != this) {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);

        if (questMarkersComplete.Length == 0) {
            questMarkersComplete = new bool[questMarkerNames.Length];
        } else {
            // if we have added to the completed quests in the editor
            // we are testing something so, leave those quests as they are
            // and fill in the rest with false if needed
            bool[] updatedQuestMarkersComplete = new bool[questMarkerNames.Length];
            for (int i = 0; i < questMarkerNames.Length; i++) {
                if (i < questMarkersComplete.Length && questMarkersComplete[i]) {
                    updatedQuestMarkersComplete[i] = true;
                    MarkQuestComplete(questMarkerNames[i]);
                } else {
                    updatedQuestMarkersComplete[i] = false;
                }
            }
            questMarkersComplete = updatedQuestMarkersComplete;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void TestSaveQuestData() {
        if (Input.GetKeyDown(KeyCode.O)) {
            SaveQuestData();
        }
    }

    void TestLoadQuestData() {
        if (Input.GetKeyDown(KeyCode.P)) {
            LoadQuestData();
        }
    }

    public void TestSetQuest() {
        string questName = "quest test";

        if (Input.GetKeyDown(KeyCode.Q)) {
            MarkQuestComplete(questName);
            bool questStatus = CheckIfComplete(questName);
            Debug.Log("Quest Status: " + questStatus);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            MarkQuestIncomplete(questName);
            bool questStatus = CheckIfComplete(questName);
            Debug.Log("Quest Status: " + questStatus);
        } 
    }

    public int GetQuestNumber(string questToCheck) {
        for (int i = 0; i < questMarkerNames.Length; i++) {
            if (questToCheck == questMarkerNames[i]) {
                return i;
            }
        }
        return -1;
    }

    public bool CheckIfComplete(string questToCheck) {
        int questNumber = GetQuestNumber(questToCheck);
        if (questNumber != -1) {
            return questMarkersComplete[questNumber];
        }

        return false;
    }

    public void MarkQuestComplete(string questToMark) {
        int questNumber = GetQuestNumber(questToMark);
        if (questNumber != -1) {
            questMarkersComplete[questNumber] = true;
        }
        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark) {
        int questNumber = GetQuestNumber(questToMark);
        if (questNumber != -1) {
            questMarkersComplete[questNumber] = false;
        }
        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects() {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();

        if (questObjects.Length > 0) {
            for (int i = 0; i < questObjects.Length; i++) {
                questObjects[i].CheckCompletion();
            }
        }
    }

    public void SaveQuestData() {
        for (int i = 0; i < questMarkersComplete.Length; i++) {
            if (questMarkersComplete[i]) {
               PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 1); 
            } else {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 0);
            }
        }
    }

    public void LoadQuestData() {
        for (int i = 0; i < questMarkersComplete.Length; i++) {
            int valueToSet = 0;
            if (PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i])) {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]);
            }

            if (valueToSet == 1) {
                questMarkersComplete[i] = true;
            } else {
                questMarkersComplete[i] = false;
            }
        }
    }
}
