using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrialMenuManager : MonoBehaviour {
    public static CombatTrialMenuManager instance;

    public GameObject combatTrialMenu;

    public List<BattleTypes> combatTrials = new List<BattleTypes>();
    public CombatTrialButton[] trialButtons;

    private bool canActivate;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            if (instance != this) {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.gameMenuOpen || GameManager.instance.dialogueActive) {
            return;
        }

        if (canActivate && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space)) && !combatTrialMenu.activeInHierarchy) {
            OpenCombatTrialMenu();
        } else if (canActivate && Input.GetKeyUp(KeyCode.Space) && combatTrialMenu.activeInHierarchy) {
            CloseCombatTrialMenu();
        }
    }

    public void OpenCombatTrialMenu() {
        ShowCombatTrialButtons();

        combatTrialMenu.SetActive(true);
        GameManager.instance.combatTrialMenuActive = true;
    }

    public void CloseCombatTrialMenu() {
        combatTrialMenu.SetActive(false);
        GameManager.instance.combatTrialMenuActive = false;
    }

    void ShowCombatTrialButtons() {
        for (int i = 0; i < trialButtons.Length; ++i) {
            if (i < combatTrials.Count) {
                trialButtons[i].gameObject.SetActive(true);
                trialButtons[i].buttonText.text = combatTrials[i].battleName;
            } else {
                trialButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = true;
        }
    }
}
