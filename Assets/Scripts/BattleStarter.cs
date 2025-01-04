using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour {
    public BattleTypes[] potentialBattles;

    public bool activateOnEnter, activateOnStay, activateOnExit;

    private bool inArea;
    public float timeBetweenBattles = 10f;
    private float betweenBattleCounter;

    public bool deactivateAfterStarting;

    public bool cannotFlee;
    public bool shouldCompleteQuest;
    public string QuestToComplete;

    // Start is called before the first frame update
    void Start() {
        betweenBattleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);
    }

    // Update is called once per frame
    void Update() {
        if (inArea && PlayerController.instance.canMove) {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                betweenBattleCounter -= Time.deltaTime;
            }

            if (betweenBattleCounter <= 0) {
                betweenBattleCounter = Random.Range(timeBetweenBattles *.5f, timeBetweenBattles * 1.5f);
                StartCoroutine(StartBattle());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnEnter) {
                StartCoroutine(StartBattle());
            } else if (activateOnStay) {
                inArea = true;
            }            
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnExit) {
                StartCoroutine(StartBattle());
            } else if (activateOnStay) {
                inArea = false;
            }
        }
    }

    public IEnumerator StartBattle() {
        UIFade.instance.FadeToBlack();
        GameManager.instance.battleActive = true;

        int selectedBattle = Random.Range(0, potentialBattles.Length);

        BattleManager.instance.rewardItems = potentialBattles[selectedBattle].rewardItems;
        BattleManager.instance.rewardXP = potentialBattles[selectedBattle].rewardXP;
        BattleManager.instance.rewardGold = potentialBattles[selectedBattle].rewardGold;

        yield return new WaitForSeconds(1.5f);

        BattleManager.instance.BattleStart(potentialBattles[selectedBattle].enemies, cannotFlee);
        UIFade.instance.FadeFromBlack();

        if (deactivateAfterStarting) {
            gameObject.SetActive(false);
        }

        BattleReward.instance.markQuestComplete = shouldCompleteQuest;
        BattleReward.instance.questToMark = QuestToComplete;
    }
}
