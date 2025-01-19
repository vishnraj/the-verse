using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour {
    public static BattleReward instance;

    public Text xpText, itemText;
    public GameObject rewardScreen;

    public string[] rewardItems;
    public int xpEarned;

    public bool markQuestComplete;
    public string questToMark;

    public int goldEarned;

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {

    }

    void TestOpenRewardsScreen() {
        if (Input.GetKeyDown(KeyCode.Y)) {
            OpenRewardsScreen(54, new string[] {"Iron Sword", "Iron Armor"}, 100);
        }
    }

    public void OpenRewardsScreen(int xp, string[] rewards, int gold) {
        xpEarned = xp;
        rewardItems = rewards;
        goldEarned = gold;

        xpText.text = "Everyone earned " + xpEarned + " xp! ";
        itemText.text = "Gold Earned: " + goldEarned + "g\n";

        for (int i = 0; i < rewardItems.Length; ++i) {
            itemText.text += rewards[i] + "\n";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardsScreen() {
        for (int i = 0; i < GameManager.instance.playerStats.Length; ++i) {
            if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                GameManager.instance.playerStats[i].AddExp(xpEarned);
            }
        }

        GameManager.instance.currentGold += goldEarned;

        for (int i = 0; i < rewardItems.Length; ++i) {
            if (GameManager.instance.GetItemDetails(rewardItems[i]).isKeyItem) {
                GameManager.instance.AddKeyItem(rewardItems[i]);
            } else {
                GameManager.instance.AddItem(rewardItems[i]);
            }
        }

        rewardScreen.SetActive(false);
        GameManager.instance.battleActive = false;

        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questToMark);
        }

        if (CombatTrialMenuManager.instance != null && CombatTrialMenuManager.instance.inCombatTrials) {
            CombatTrialMenuManager.instance.OpenCombatTrialMenu();
        }
    }
}
