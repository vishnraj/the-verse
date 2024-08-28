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

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {

    }

    void TestOpenRewardsScreen() {
        if (Input.GetKeyDown(KeyCode.Y)) {
            OpenRewardsScreen(54, new string[] {"Iron Sword", "Iron Armor"});
        }
    }

    public void OpenRewardsScreen(int xp, string[] rewards) {
        xpEarned = xp;
        rewardItems = rewards;

        xpText.text = "Everyone earned " + xpEarned + " xp! ";
        itemText.text = "";

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

        for (int i = 0; i < rewardItems.Length; ++i) {
            GameManager.instance.AddItem(rewardItems[i]);
        }

        rewardScreen.SetActive(false);
        GameManager.instance.battleActive = false;

        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questToMark);
        }
    }
}
