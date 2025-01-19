using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrialMenuManager : MonoBehaviour {
    public static CombatTrialMenuManager instance;

    public GameObject combatTrialMenu;

    public List<BattleTypes> combatTrials = new List<BattleTypes>();
    public CombatTrialButton[] trialButtons;

    private bool canActivate;

    public bool inCombatTrials;

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
        if (GameManager.instance.gameMenuOpen || GameManager.instance.dialogueActive || GameManager.instance.battleActive || GameManager.instance.shopActive) {
            return;
        }

        if (canActivate && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space)) && !combatTrialMenu.activeInHierarchy) {
            StartCombatTrials();
        } else if (canActivate && Input.GetKeyUp(KeyCode.Space) && combatTrialMenu.activeInHierarchy) {
            LeaveCombatTrials();
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
                trialButtons[i].buttonValue = i;
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

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = false;
        }
    }

    public void StartTrial(int trialIndex) {
        if (trialIndex < combatTrials.Count) {
            BattleTypes selectedTrial = combatTrials[trialIndex];

            StartCoroutine(StartTrialBattle(selectedTrial));
        } else {
            Debug.LogError("Invalid trial index: " + trialIndex);
        }
    }

    public IEnumerator StartTrialBattle(BattleTypes selectedTrial) {
        UIFade.instance.FadeToBlack();
        GameManager.instance.battleActive = true;

        BattleManager.instance.rewardItems = selectedTrial.rewardItems;
        BattleManager.instance.rewardXP = selectedTrial.rewardXP;
        BattleManager.instance.rewardGold = selectedTrial.rewardGold;

        CloseCombatTrialMenu();

        yield return new WaitForSeconds(1.5f);

        BattleManager.instance.BattleStart(selectedTrial.enemies, true);
        UIFade.instance.FadeFromBlack();
    }

    public void StartCombatTrials() {
        inCombatTrials = true;
        OpenCombatTrialMenu();
    }

    public void LeaveCombatTrials() {
        inCombatTrials = false;
        CloseCombatTrialMenu();
    }
}
