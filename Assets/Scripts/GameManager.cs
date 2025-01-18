using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public CharStats[] playerStats;

    public bool gameMenuOpen, dialogueActive, fadingBetweenAreas, shopActive, battleActive, combatTrialMenuActive;

    public string[] itemsHeld;
    public string[] keyItems;
    public int[] numberOfItems;
    public Item[] referenceItems;

    public int currentGold;

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

        // Now that GameManager is setup, we can activate the
        // child gameObjects of AreaExits, which are basically
        // the AreaEntrances, which rely on GameManager for
        // state information
        AreaExit[] areaExits = FindObjectsOfType<AreaExit>();
        foreach (AreaExit area in areaExits) {
            for (int i = 0; i < area.transform.childCount; i++) {
                Transform child = area.transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (gameMenuOpen || dialogueActive || fadingBetweenAreas || shopActive || battleActive || combatTrialMenuActive) {
            PlayerController.instance.canMove = false;
        } else {
            PlayerController.instance.canMove = true;
        }
    }

    void TestSaveData() {
        if (Input.GetKeyDown(KeyCode.O)) {
            SaveData();
        }
    }

    void TestLoadData() {
        if (Input.GetKeyDown(KeyCode.P)) {
            LoadData();
        }
    }

    public void TestItemAdd() {
        if (Input.GetKeyDown(KeyCode.J)) {
            AddItem("Iron Armor");
            // AddItem("Blah");
        }
    }

    public void TestItemRemove() {
        if (Input.GetKeyDown(KeyCode.K)) {
            RemoveItem("Health Potion");
            RemoveItem("Iron Armor");
            // AddItem("Blah");
        }
    }

    public Item GetItemDetails(string itemName) {
        for (int i = 0; i < referenceItems.Length; i++) {
            if (referenceItems[i].itemName == itemName) {
                return referenceItems[i];
            }
        }
        return null;
    }

    public void SortKeyItems() {
        bool done = false;
        while (!done) {
            bool itemMoved = false;
            for (int i = 0; i < keyItems.Length - 1; i++) {
                if (keyItems[i] == "") {
                    keyItems[i] = keyItems[i + 1];
                    keyItems[i + 1] = "";

                    if (keyItems[i] != "") {
                        itemMoved = true;
                    }
                }
            }

            if (!itemMoved) {
                done = true;
            }
        }
    }

    public void SortItems() {
        bool done = false;
        while (!done) {
            bool itemMoved = false;
            for (int i = 0; i < itemsHeld.Length - 1; i++) {
                if (itemsHeld[i] == "") {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "") {
                        itemMoved = true;
                    }
                }
            }

            if (!itemMoved) {
                done = true;
            }
        }
    }

    public void AddKeyItem(string itemToAdd) {
        bool foundSpace = false;

        for (int i = 0; i < keyItems.Length; i++) {
            if (keyItems[i] == "") {
                keyItems[i] = itemToAdd;
                foundSpace = true;
                break;
            }
        }

        if (!foundSpace) {
            Debug.LogError("Key Items are full");
        }
    }

    public bool RemoveKeyItem(string itemToRemove) {
        bool foundItem = false;

        for (int i = 0; i < keyItems.Length; i++) {
            if (keyItems[i] == itemToRemove) {
                keyItems[i] = "";
                foundItem = true;
                break;
            }
        }

        return foundItem;
    }

    public void AddItem(string itemToAdd) {
        int newItemPosition = 0;
        bool foundSpace = false;

        for (int i = 0; i < itemsHeld.Length && !foundSpace; i++) {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd) {
                newItemPosition = i;
                foundSpace = true;
                break;
            }
        }

        if (foundSpace) {
            bool itemExists = false;
            for (int i = 0; i < referenceItems.Length; i++) {
                if (referenceItems[i].itemName == itemToAdd) {
                    itemExists = true;
                    break;
                }
            }

            if (itemExists) {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;
            } else {
                Debug.LogError("No reference item found for " + itemToAdd);
            }
        }

        GameMenu.instance.ShowItems();
    }

    public bool RemoveItem(string itemToRemove) {
        bool foundItem = false;
        int itemPosition = 0;
        for (int i = 0; i < itemsHeld.Length && !foundItem; i++) {
            if (itemsHeld[i] == itemToRemove) {
                itemPosition = i;
                foundItem = true;
            }
        }

        if (foundItem) {
            numberOfItems[itemPosition]--;
            if (numberOfItems[itemPosition] <= 0) {
                itemsHeld[itemPosition] = "";
                numberOfItems[itemPosition] = 0;
            }
        }

        GameMenu.instance.ShowItems();

        return foundItem;
    }

    public void SaveData() {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

        for (int i = 0; i < playerStats.Length; i++) {
            if (playerStats[i].gameObject.activeInHierarchy) {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 1);
            } else {
               PlayerPrefs.SetInt("Player_" + playerStats[i].charName  + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentExp", playerStats[i].currentExp);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentMP", playerStats[i].currentMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxMP", playerStats[i].maxMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Strength", playerStats[i].strength);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Defense", playerStats[i].defense);
            PlayerPrefs.SetInt("Player_"  + playerStats[i].charName   +  "_WpnPwr", playerStats[i].wpnPwr);
            PlayerPrefs.SetInt("Player_"  + playerStats[i].charName   +  "_ArmrPwr", playerStats[i].armrPwr);
            PlayerPrefs.SetString("Player_"  + playerStats[i].charName   +  "_EquippedWpn", playerStats[i].equippedWpn);
            PlayerPrefs.SetString("Player_"  + playerStats[i].charName   +  "_EquippedArmr", playerStats[i].equippedArmr);
        }

        for (int i = 0; i < itemsHeld.Length; i++) {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
            PlayerPrefs.SetString("KeyItem_" + i, keyItems[i]);
        }

        PlayerPrefs.SetInt("CurrentGold", currentGold);
    }

    public void LoadData() {
        SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene"));

        PlayerController.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Position_x"),
            PlayerPrefs.GetFloat("Player_Position_y"),
            PlayerPrefs.GetFloat("Player_Position_z")
        );

        for (int i = 0; i < playerStats.Length; i++) {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 0) {
                playerStats[i].gameObject.SetActive(false);
            } else {
                playerStats[i].gameObject.SetActive(true);
            }

            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Level");
            playerStats[i].currentExp = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentExp");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxHP");
            playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentMP");
            playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxMP");
            playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Strength");
            playerStats[i].defense = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Defense");
            playerStats[i].wpnPwr = PlayerPrefs.GetInt("Player_"  + playerStats[i].charName   +  "_WpnPwr");
            playerStats[i].armrPwr = PlayerPrefs.GetInt("Player_"  + playerStats[i].charName   +  "_ArmrPwr");
            playerStats[i].equippedWpn = PlayerPrefs.GetString("Player_"  + playerStats[i].charName   +  "_EquippedWpn");
            playerStats[i].equippedArmr = PlayerPrefs.GetString("Player_"  + playerStats[i].charName   +  "_EquippedArmr");
        }

        for (int i = 0; i < itemsHeld.Length; i++) {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
            keyItems[i] = PlayerPrefs.GetString("KeyItem_" + i);
        }

        currentGold = PlayerPrefs.GetInt("CurrentGold");
    }
}
