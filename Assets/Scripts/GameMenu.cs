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

    public Image statusImage;

    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText;

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
        if (Input.GetButtonDown("Fire2")) {
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
            }
        }

        itemCharChoiceMenu.SetActive(false);
    }

    public void OpenMenu() {
        if (GameManager.instance.battleActive) {
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

    public void SelectItem(Item newItem) {
       activeItem = newItem;

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
}
