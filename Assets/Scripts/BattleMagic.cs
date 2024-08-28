using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagic : MonoBehaviour {
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Press() {
        if (BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP < spellCost) {
            // let player know there is not enough MP to cast this spell
            BattleManager.instance.battleNotice.displayText.text = "Not enough MP!";
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.magicMenu.SetActive(false);
            return;
        }

        BattleManager.instance.magicMenu.SetActive(false);
        BattleManager.instance.OpenTargetMenu(spellName);

        BattleChar currentBattleChar = BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn];
        currentBattleChar.currentMP -= spellCost;

        if (currentBattleChar.currentMP < 0) {
           currentBattleChar.currentMP = 0;
        }
        for (int i = 0; i < GameManager.instance.playerStats.Length; ++i) {
            if (currentBattleChar.charName == GameManager.instance.playerStats[i].charName) {
                GameManager.instance.playerStats[i].currentMP = currentBattleChar.currentMP;
                break;
            }
        }
    }
}
