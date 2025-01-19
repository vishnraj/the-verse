using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {
    public static BattleManager instance;

    private bool battleActive;

    public GameObject battleScene;
    public int battleThemeIndex = -1;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;

    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;

    public DamageNumber damageNumber;

    public Text[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;
    
    public GameObject magicMenu;
    public BattleMagic[] magicButtons;

    public BattleNotification battleNotice;

    public int chanceToFlee = 35;
    private bool fleeing;

    public GameObject battleItemsMenu;
    public ItemButton[] itemButtons;
    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText;

    public string gameOverScene;
    private bool gameOverSet;

    public int rewardXP;
    public string[] rewardItems;
    public int rewardGold;

    public bool cannotFlee;

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

        DontDestroyOnLoad(gameObject);

        if (battleThemeIndex == -1) {
            battleThemeIndex = 0;
        }
    }

    // Update is called once per frame
    void Update() {
        if (battleActive) {
            if (turnWaiting) {
                if (activeBattlers[currentTurn].isPlayer) {
                    uiButtonsHolder.SetActive(true);
                } else {
                    uiButtonsHolder.SetActive(false);

                    StartCoroutine(EnemyMoveCo());
                }
            }
        }
    }

    public void TestBattleStart(){
        if (Input.GetKeyDown(KeyCode.T)) {
            BattleStart(new string[]{"Eyeball", "Spider", "Skeleton"}, false);
        }
    }

    public void TestIncrementTurn(){
        if (Input.GetKeyDown(KeyCode.N)) {
            NextTurn();
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee){
        if (battleActive) {
            return;
        }

        if (CombatTrialMenuManager.instance != null && CombatTrialMenuManager.instance.gameObject.activeInHierarchy) {
            inCombatTrials = true;
            CombatTrialMenuManager.instance.CloseCombatTrialMenu();
        } else {
            inCombatTrials = false;
        }

        cannotFlee = setCannotFlee;

        ActivateBattle();

        for (int i = 0; i < playerPositions.Length; ++i) {
            if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                for (int j = 0; j < playerPrefabs.Length; ++j) {
                    if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName) {
                        BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                        newPlayer.transform.parent = playerPositions[i];
                        activeBattlers.Add(newPlayer);

                        CharStats playerStats = GameManager.instance.playerStats[i];
                        activeBattlers[i].currentHP = playerStats.currentHP;
                        activeBattlers[i].maxHP = playerStats.maxHP;
                        activeBattlers[i].currentMP = playerStats.currentMP;
                        activeBattlers[i].maxMP = playerStats.maxMP;
                        activeBattlers[i].strength = playerStats.strength;
                        activeBattlers[i].defense = playerStats.defense;
                        activeBattlers[i].wpnPwr = playerStats.wpnPwr;
                        activeBattlers[i].armrPwr = playerStats.armrPwr;

                        if (activeBattlers[i].currentHP <= 0) {
                           activeBattlers[i].currentHP = 0;
                           activeBattlers[i].spriteRenderer.sprite = activeBattlers[i].deadSprite;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < enemiesToSpawn.Length; ++i) {
            if (enemiesToSpawn[i] != "") {
                for (int j = 0; j < enemyPrefabs.Length; ++j) {
                    if (enemyPrefabs[j].charName == enemiesToSpawn[i]) {
                        BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                        newEnemy.transform.parent  = enemyPositions[i];
                        activeBattlers.Add(newEnemy);
                    }
                }    
            }
        }

        turnWaiting = true;
        currentTurn = 0;

        while (activeBattlers[currentTurn].currentHP == 0) {
            ++currentTurn;
            if (currentTurn >= activeBattlers.Count) {
                currentTurn = 0;
            }
        }

        UpdateUIStats();
    }

    public void NextTurn() {
        ++currentTurn;
        if (currentTurn >= activeBattlers.Count) {
            currentTurn = 0;
        }

        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle() {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; ++i) {
            if (activeBattlers[i].currentHP < 0) {
                activeBattlers[i].currentHP = 0;
            }

            if (activeBattlers[i].currentHP == 0) {
                if (activeBattlers[i].isPlayer) {
                    activeBattlers[i].spriteRenderer.sprite = activeBattlers[i].deadSprite;
                } else {
                    activeBattlers[i].EnemyFade();
                }
            } else {
                if (activeBattlers[i].isPlayer) {
                    allPlayersDead = false;
                    activeBattlers[i].spriteRenderer.sprite = activeBattlers[i].aliveSprite;
                } else {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead) {
            if (allEnemiesDead) {
                // end battle in victory
                StartCoroutine(EndBattle());
            } else {
                // end battle scene in game over
                StartCoroutine(GameOver());
            }
        } else {
            while (activeBattlers[currentTurn].currentHP == 0) {
                ++currentTurn;
                if (currentTurn >= activeBattlers.Count) {
                    currentTurn = 0;
                }
            }
        }
    }

    public void ActivateBattle() {
        battleActive = true;
        GameManager.instance.battleActive = true;
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
        battleScene.SetActive(true);
        battleScene.GetComponentInChildren<SpriteRenderer>().sprite = Camera.main.GetComponent<CameraController>().battleBackground;
        AudioManager.instance.PlayBGM(battleThemeIndex);
    }

    public void DeactivateBattle() {
        StartDeactivateBattle();
        CleanupDeactivateBattle();
    }

    public void StartDeactivateBattle() {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
    }

    public void CleanupDeactivateBattle() {
        for (int i = 0; i < activeBattlers.Count; ++i) {
            if (activeBattlers[i].isPlayer) {
                // set player stats to what they should be from the battle
                for (int j = 0; j < GameManager.instance.playerStats.Length; ++j) {
                    if (GameManager.instance.playerStats[j].charName == activeBattlers[i].charName) {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        // reset battle data
        activeBattlers.Clear();
        currentTurn = 0;
        battleScene.SetActive(false);

        if (fleeing) {
            GameManager.instance.battleActive = false;
            fleeing = false;
        } else if (gameOverSet) {
            GameManager.instance.battleActive = false; 
            gameOverSet = false;
        } else {
            // we should only be opening this screen in the case of victory
            BattleReward.instance.OpenRewardsScreen(rewardXP, rewardItems, rewardGold);
        }
        
        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

    public IEnumerator EnemyMoveCo() {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn(); // provided the player can't interact with the UI to update currentTurn
                    // which should be the case by this point, this running in a separate coroutine
                    // shouldn't be an issue, but if the player could update currentTurn, while this is
                    // is happening, then it would lead to a race condition
    }

    public void EnemyAttack() {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; ++i) {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) {
                players.Add(i);
            }   
        }

        if (players.Count == 0) {
            return; // nothing to do here, battle would just end
        }

        // this is basically the real enemy AI, which can be updated later
        int selectedTarget = players[Random.Range(0, players.Count)];
        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;
        for (int i = 0; i < movesList.Length; ++i) {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]) {
                Instantiate(movesList[i].effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower) {
        float atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].wpnPwr;
        float defPwr = activeBattlers[target].defense + activeBattlers[target].armrPwr;

        float damageCalc = atkPwr / defPwr * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);

        activeBattlers[target].currentHP -=  damageToGive;
        if (activeBattlers[target].currentHP < 0) {
           activeBattlers[target].currentHP = 0;
        }
        for (int i = 0; i < GameManager.instance.playerStats.Length; ++i) {
            if (activeBattlers[target].charName == GameManager.instance.playerStats[i].charName) {
                GameManager.instance.playerStats[i].currentHP = activeBattlers[target].currentHP;
                break;
            }
        }

        Instantiate(damageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damageToGive);

        UpdateUIStats();
    }

    public void UpdateUIStats() {
        for (int i = 0; i < playerName.Length; ++i) {
            if (activeBattlers.Count > i) {
                if (activeBattlers[i].isPlayer) {
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                } else {
                    playerName[i].gameObject.SetActive(false);
                }
            } else {
               playerName[i].gameObject.SetActive(false); 
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget) {
        BattleChar currentBattleChar = activeBattlers[currentTurn];

        int movePower = 0;
        int moveMP = 0;
        for (int i = 0; i < movesList.Length; ++i) {
            if (movesList[i].moveName == moveName) {
                Instantiate(movesList[i].effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
                moveMP = movesList[i].moveCost;
            }
        }

        currentBattleChar.currentMP -= moveMP;

        if (currentBattleChar.currentMP < 0) {
           currentBattleChar.currentMP = 0;
        }
        for (int i = 0; i < GameManager.instance.playerStats.Length; ++i) {
            if (currentBattleChar.charName == GameManager.instance.playerStats[i].charName) {
                GameManager.instance.playerStats[i].currentMP = currentBattleChar.currentMP;
                break;
            }
        }

        // called enemyAttackEffect, but this is just the particle effect that we create to highlight which BattleChar is attacking
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        NextTurn();
    }

    public void OpenTargetMenu(string moveName) {
        targetMenu.SetActive(true);

        List<int> Enemies = new List<int>();
        for (int i = 0; i < activeBattlers.Count; ++i) {
            if (!activeBattlers[i].isPlayer) {
                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; ++i) {
            if (Enemies.Count > i && activeBattlers[Enemies[i]].currentHP > 0) {
                targetButtons[i].gameObject.SetActive(true);
                
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = Enemies[i];
                targetButtons[i].targetName.text = activeBattlers[Enemies[i]].charName;
            } else {
                targetButtons[i].gameObject.SetActive(false);                
            }
        }
    }

    public void CloseTargetMenu() {
        targetMenu.SetActive(false);
    }

    public void OpenMagicMenu() {
        magicMenu.SetActive(true);
        for (int i = 0; i < magicButtons.Length; ++i) {
            if (activeBattlers[currentTurn].movesAvailable.Length > i) {
                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                for (int j = 0; j < movesList.Length; ++j) {
                    if (movesList[j].moveName == magicButtons[i].spellName) {
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            } else {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CloseMagicMenu() {
        magicMenu.SetActive(false);
    }

    public void Flee() {
        if (cannotFlee) {
            battleNotice.displayText.text = "Cannot flee this battle!";
            battleNotice.Activate();
            return; 
        }

        int fleeSuccess = Random.Range(0, 100);

        if (fleeSuccess < chanceToFlee) {
            // end the battle
            fleeing = true;
            StartCoroutine(EndBattle());
        } else {
            NextTurn();
            battleNotice.displayText.text = "Couldn't escape!";
            battleNotice.Activate();
        }
    }

    public void OpenBattleItems() {
        battleItemsMenu.SetActive(true);
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

    public void CloseBattleItems() {
        battleItemsMenu.SetActive(false);
    }

    public void SelectItem(Item newItem) {
       activeItem = newItem;

        if (activeItem.isItem) {
            useButtonText.text = "Use";
        } else if (activeItem.isWeapon || activeItem.isArmor) {
            useButtonText.text = "Unusable";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
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
            // might also be useful to say no item was selected with a battleNotice
            CloseItemCharChoice();
            battleItemsMenu.SetActive(false);
            return;
        } else if (activeItem.isWeapon || activeItem.isArmor) {
            // might also be useful to say can't equip in battle with a battleNotice
            CloseItemCharChoice();
            battleItemsMenu.SetActive(false);
            return;
        }

        activeItem.Use(selectChar);
        battleItemsMenu.SetActive(false);

        CharStats selectedPlayer = GameManager.instance.playerStats[selectChar];
        for (int i = 0; i < activeBattlers.Count; ++i) {
            if (activeBattlers[i].charName == selectedPlayer.charName) {
                activeBattlers[i].currentHP = selectedPlayer.currentHP;
                activeBattlers[i].currentMP = selectedPlayer.currentMP;
            }
        }
        UpdateUIStats();

        NextTurn();
    }

    public IEnumerator EndBattle() {
        StartDeactivateBattle();

        yield return new WaitForSeconds(.5f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        CleanupDeactivateBattle();

        UIFade.instance.FadeFromBlack();
    }

    public IEnumerator GameOver() {
        StartDeactivateBattle();
        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);
        gameOverSet = true;
        CleanupDeactivateBattle();

        SceneManager.LoadScene(gameOverScene);
    }
}
