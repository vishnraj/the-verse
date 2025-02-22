using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon;
    public bool isArmor;
    public bool isKeyItem;

    [Header("Item Details")]
    public string itemName;
    public string description;
    public int value;
    public Sprite itemSprite;

    [Header("Item Effects")]
    public int amountToChange;
    public bool affectHP;
    public bool affectMP;
    public bool affectStr;

    [Header("Weapon/Armor Details")]
    public int weaponStrength;
    public int armorStrength;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Use(int charToUseOn) {
       CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];

        if (isItem) {
            if (affectHP) {
                selectedChar.currentHP += amountToChange;
                if (selectedChar.currentHP > selectedChar.maxHP) {
                    selectedChar.currentHP = selectedChar.maxHP;
                }
            } else if (affectMP) {
                selectedChar.currentMP += amountToChange;
                if (selectedChar.currentMP > selectedChar.maxMP) {
                    selectedChar.currentMP = selectedChar.maxMP;
                }
            } else if (affectStr) {
                selectedChar.strength += amountToChange;
            }
        } else if (isWeapon) {
            if (selectedChar.equippedWpn != "") {
                GameManager.instance.AddItem(selectedChar.equippedWpn);                
            }

            selectedChar.equippedWpn = itemName;
            selectedChar.wpnPwr = weaponStrength;
        } else if (isArmor) {
            if (selectedChar.equippedArmr  != "") {
                GameManager.instance.AddItem(selectedChar.equippedArmr);
            }

            selectedChar.equippedArmr = itemName;
            selectedChar.armrPwr = armorStrength;
        }

        GameManager.instance.RemoveItem(itemName);
    }
}
