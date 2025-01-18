using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject menu;
    public GameObject[] windows;

    private CharStats[] playerStats;

    public Text[] nameText;
    public Text[] hpText;
    public Text[] mpText;
    public Text[] lvlText;
    public Text[] expText;

    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

    public GameObject[] statusButtons;

    public Text statusName;
    public Text statusHP;
    public Text statusMP;
    public Text statusStr;
    public Text statusDef;
    public Text statusWpnEqpd;
    public Text statusWpnPwr;
    public Text statusArmrEqpd;
    public Text statusArmrPwr;
    public Text statusExp;

    private int currentStatChar = -1;

    public Image statusImage;

    public ItemButton[] itemButtons, keyItemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText, keyItemName, keyItemDescription;

    public static GameMenu instance;

    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;

    public Text goldText;

    public string mainMenuName;

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
    }

    // Update is called once per frame
    void Update() {
        if (!Shop.instance.shopMenu.activeInHierarchy && !GameManager.instance.combatTrialMenuActive && (Input.GetButtonDown("Fire2") || Input.GetKeyUp(KeyCode.M))) {
            if (menu.activeInHierarchy) {
                CloseMenu();
            } else {
                OpenMenu();
            }
        }
    }

    public void UpdateMainStats() {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++) {
            if (playerStats[i].gameObject.activeInHierarchy) {
                charStatHolder[i].SetActive(true);

                nameText[i].text = playerStats[i].charName;
                hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                lvlText[i].text = "Lvl: " + playerStats[i].playerLevel;
                expText[i].text = "" + playerStats[i].currentExp + "/" + Mathf.FloorToInt(playerStats[i].expToNextLevel[playerStats[i].playerLevel]);
                expSlider[i].maxValue = Mathf.FloorToInt(playerStats[i].expToNextLevel[playerStats[i].playerLevel]);
                expSlider[i].value = playerStats[i].currentExp;
                charImage[i].sprite = playerStats[i].charImage;
            } else {
                charStatHolder[i].SetActive(false);
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() + "g";
    }

    public void ToggleWindow(int windowNumber) {
        UpdateMainStats();

        for (int i = 0; i < windows.Length; i++) {
            if (windowNumber == i) {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            } else {
                windows[i].SetActive(false);

                if (windows[i].name == "Status Window") {
                    currentStatChar = -1;
                }
            }
        }

        itemCharChoiceMenu.SetActive(false);
    }

    public void OpenMenu() {
        if (GameManager.instance.battleActive || GameManager.instance.dialogueActive) {
            return; // only battle menu should be used when in battle
        }

        menu.SetActive(true);
        UpdateMainStats();
        GameManager.instance.gameMenuOpen = true;
        AudioManager.instance.PlaySFX(5);
    }

    public void CloseMenu() {
        for (int i = 0; i < windows.Length; i++) {
            windows[i].SetActive(false);
        }
        menu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;

        itemCharChoiceMenu.SetActive(false);

        currentStatChar = -1;
    }

    public void OpenStatus() {
        UpdateMainStats();

        // update status info that is shown
        StatusChar(0);

        for (int i = 0; i < statusButtons.Length; i++)  {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName;
        }
    }

    public void QuitGame() {
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
        SceneManager.LoadScene(mainMenuName);
    }

    public void StatusChar(int selected) {
        CharStats playerStat = playerStats[selected];

        statusName.text = playerStat.charName;
        statusHP.text = "" + playerStat.currentHP + "/" + playerStat.maxHP;
        statusMP.text = "" + playerStat.currentMP + "/" + playerStat.maxMP;
        statusStr.text = playerStat.strength.ToString();
        statusDef.text = playerStat.defense.ToString();

        if (playerStat.equippedWpn != "") {
            statusWpnEqpd.text = playerStat.equippedWpn;
        } else {
            statusWpnEqpd.text = "None";
        }

        statusWpnPwr.text = playerStat.wpnPwr.ToString();

        if (playerStat.equippedArmr  != "") {
            statusArmrEqpd.text = playerStat.equippedArmr;
        } else {
            statusArmrEqpd.text = "None";
        }

        statusArmrPwr.text = playerStat.armrPwr.ToString();

        int expToNextLevel = (int)playerStat.expToNextLevel[playerStat.playerLevel] - playerStat.currentExp;
        statusExp.text = expToNextLevel.ToString();

        statusImage.sprite = playerStat.charImage;

        currentStatChar = selected;
    }

    public void ShowItems() {
        GameManager.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++)   {
            itemButtons[i].buttonValue = i;
            
            if (GameManager.instance.itemsHeld[i] != "") {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            } else {
               itemButtons[i].buttonImage.gameObject.SetActive(false);
               itemButtons[i].amountText.text = ""; 
            }
        }
    }

    public void ShowKeyItems() {
        GameManager.instance.SortKeyItems();

        for (int i = 0; i < keyItemButtons.Length; i++)   {
            keyItemButtons[i].buttonValue = i;
            
            if (GameManager.instance.keyItems[i] != "") {
                keyItemButtons[i].buttonImage.gameObject.SetActive(true);
                keyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.keyItems[i]).itemSprite;
            } else {
               keyItemButtons[i].buttonImage.gameObject.SetActive(false);
            }

            keyItemButtons[i].amountText.text = ""; // key items don't have amounts
        }
    }

    public void SelectItem(Item newItem) {
       activeItem = newItem;

        if (activeItem.isKeyItem) {
            keyItemName.text = activeItem.itemName;
            keyItemDescription.text = activeItem.description;
            return;
        }

        if (activeItem.isItem) {
            useButtonText.text = "Use";
        } else if (activeItem.isWeapon || activeItem.isArmor) {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }

    public void DiscardItem() {
        if (activeItem != null) {
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OptnItemCharChoice() {
        itemCharChoiceMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++) {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharChoice() {
        itemCharChoiceMenu.SetActive(false);
    }

    public void UseItem(int selectChar) {
        if (activeItem == null) {
            CloseItemCharChoice();
            return;
        }

        activeItem.Use(selectChar);
        CloseItemCharChoice();
    }

    public void SaveGame() {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound() {
        AudioManager.instance.PlaySFX(4);
    }

    public void UnequipWeapon() {
        if (currentStatChar == -1) {
            return;
        }

        CharStats selectedChar = GameManager.instance.playerStats[currentStatChar];
        if (selectedChar.equippedWpn == "") {
            return;
        }

        Item wpnDetails = GameManager.instance.GetItemDetails(selectedChar.equippedWpn);
        if (wpnDetails == null) {
            Debug.LogError("No item found for " + selectedChar.equippedWpn);
            return;
        }

        selectedChar.equippedWpn = "";
        if (wpnDetails.isWeapon) {
            selectedChar.wpnPwr -= wpnDetails.weaponStrength;
        } else {
            Debug.LogError("Item is not weapon: " + selectedChar.equippedWpn);
            return; 
        }

        GameManager.instance.AddItem(wpnDetails.itemName);
        StatusChar(currentStatChar);
    }

    public void UnequipArmor() {
        if (currentStatChar == -1) {
            return;
        }

        CharStats selectedChar = GameManager.instance.playerStats[currentStatChar];
        if (selectedChar.equippedArmr == "") {
            return;
        }

        Item armorDetails = GameManager.instance.GetItemDetails(selectedChar.equippedArmr);
        if (armorDetails == null) {
            Debug.LogError("No item found for " + selectedChar.equippedArmr);
            return;
        }

        selectedChar.equippedArmr = "";
        if (armorDetails.isArmor) {
            selectedChar.armrPwr -= armorDetails.armorStrength;
        } else {
            Debug.LogError("Item is not armor: " + selectedChar.equippedArmr);
            return; 
        }

        GameManager.instance.AddItem(armorDetails.itemName);
        StatusChar(currentStatChar);
    }
}
