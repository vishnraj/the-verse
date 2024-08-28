using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour {
    public string charName;

    public int playerLevel;
    public int maxLevel;

    public int currentExp;
    public int baseEXP;
    public float levelMultiplierExp;
    public float minimumIncreaseValueExp;
    public float[] expToNextLevel;
    
    public int currentHP;
    public int maxHP;
    public float levelMultiplierHP;

    public int currentMP;
    public int maxMP;
    public int baseBonusMP;
    public float levelMultiplierMP;
    public float minimumIncreaseValueMP;
    public float[] mpLvlBonus;

    public int strength;
    public int defense;
    public int wpnPwr;
    public int armrPwr;

    public string equippedWpn;
    public string equippedArmr;

    public Sprite charImage;

    public bool testMode;

    // Start is called before the first frame update
    void Start() {
        if (playerLevel == 0) {
            playerLevel = 1;
        }
        if (maxHP == 0) {
            maxHP = 100;
        }
        if (maxMP == 0) {
            maxMP = 30;
        }
        if (maxLevel == 0) {
            maxLevel = 100;
        }
        if (baseEXP == 0) {
            baseEXP = 1000;
        }
        if (baseBonusMP == 0) {
            baseBonusMP = 3;
        }
        if (levelMultiplierExp == 0) {
            levelMultiplierExp = 1.05f;
        }
        if (levelMultiplierHP == 0) {
            levelMultiplierHP = 1.05f;
        }
        if (levelMultiplierMP == 0) {
            levelMultiplierMP = 1f;
        }
        if (minimumIncreaseValueExp == 0) {
            minimumIncreaseValueExp = 0;
        }
        if (minimumIncreaseValueMP == 0) {
            minimumIncreaseValueMP = .025f;
        }
        if (strength == 0) {
            strength = 15;
        }
        if (defense == 0) {
            defense = 12;
        }

        InitProgressionArray(baseEXP, ref expToNextLevel, maxLevel, levelMultiplierExp, minimumIncreaseValueExp);
        InitProgressionArray(baseBonusMP, ref mpLvlBonus, maxLevel, levelMultiplierMP, minimumIncreaseValueMP);
    }

    // Update is called once per frame
    void Update() {
        if (testMode) {
            TestLevelIncrease();
        }
    }

    public void InitProgressionArray(int baseValue, ref float[] progressionArray, int maxProgressionLevels, float progressionMultiplier, float minimumIncreaseValue) {
        progressionArray = new float[maxProgressionLevels];
        progressionArray[1] = baseValue;
        for (int i = 2; i < progressionArray.Length; ++i) {
            progressionArray[i] += progressionArray[i - 1] * (progressionMultiplier + minimumIncreaseValue);
        }
    }

    public void AddExp(int exp) {
        currentExp += exp;
        if (playerLevel < maxLevel) {
            int expToNextLevelFloor = Mathf.FloorToInt(expToNextLevel[playerLevel]);

            while (currentExp >= expToNextLevelFloor) {
                currentExp -= expToNextLevelFloor;
                ++playerLevel;

                // this shouldn't happen, but we'll catch it just in case
                // so we don't index on a level that doesn't exist
                if (playerLevel >= maxLevel) {
                    currentExp = 0;
                    playerLevel = maxLevel;
                }

                // decide which stats to increase on level up
                StatIncreaseBasic(playerLevel);
                expToNextLevelFloor = Mathf.FloorToInt(expToNextLevel[playerLevel]);

                // this shouldn't happen, but we'll catch it just in case
                // so we don't keep looping here
                if (playerLevel >= maxLevel) {
                    break;
                }
            }
        }

        if (playerLevel >= maxLevel) {
            currentExp = 0;
            playerLevel = maxLevel;
        }
    }

    public void StatIncreaseBasic(int playerLevel) {
        if (playerLevel % 2 == 0) {
            ++strength;
        } else {
            ++defense;
        }

        maxHP = Mathf.FloorToInt(maxHP * levelMultiplierHP);
        currentHP = maxHP;
        if (playerLevel < maxLevel) {
            maxMP += Mathf.FloorToInt(mpLvlBonus[playerLevel]);
        }
        currentMP = maxMP;
    }

    // test functions
    public void TestLevelIncrease() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AddExp(500);
        }
    }
}
