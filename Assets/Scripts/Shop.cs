using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    public static Shop instance;

    public GameObject shopMenu;
    public GameObject buyMenu;
    public GameObject sellMenu;

    public Text goldText;

    public string[] itemsForSale;

    public ItemButton[] buyItemButtons;
    public ItemButton[] sellItemButtons;

    public Item selectedItem;
    public Text buyItemName, buyItemDescription, buyItemValue;
    public Text sellItemName, sellItemDescription, sellItemValue;

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

    }

    public void TestOpenShop() {
        if (Input.GetKeyDown(KeyCode.K) && !shopMenu.activeInHierarchy) {
            OpenShop();
        }
    }

    public void OpenShop() {
        shopMenu.SetActive(true);
        OpenBuyMenu();
        GameManager.instance.shopActive = true;
        goldText.text = GameManager.instance.currentGold.ToString() + "g";
    }

    public void CloseShop() {
        sellMenu.SetActive(false);
        buyMenu.SetActive(false);
        shopMenu.SetActive(false);
        GameManager.instance.shopActive = false;
    }

    public void OpenBuyMenu() {
        if (buyMenu.activeInHierarchy) {
            return;
        }

        SelectBuyItem(GameManager.instance.GetItemDetails(itemsForSale[0])); // just show first item in the list by default

        buyMenu.SetActive(true);
        sellMenu.SetActive(false);

        for (int i = 0; i < buyItemButtons.Length; i++) {
            buyItemButtons[i].buttonValue = i;

            if (itemsForSale[i] != "") {
                buyItemButtons[i].buttonImage.gameObject.SetActive(true);
                buyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(itemsForSale[i]).itemSprite;
                buyItemButtons[i].amountText.text = "";
            } else {
                buyItemButtons[i].buttonImage.gameObject.SetActive(false);
                buyItemButtons[i].amountText.text = "";
            }
        }
    }

    public void OpenSellMenu() {
        if (sellMenu.activeInHierarchy) {
            return;
        }

        GameManager.instance.SortItems();
        SelectSellItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[0])); // just show first item in the list by default

        buyMenu.SetActive(false);
        sellMenu.SetActive(true);
        ShowSellItems();
    }

    void ShowSellItems() {
       GameManager.instance.SortItems();
       for (int i = 0; i < sellItemButtons.Length; i++) {
            sellItemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "") {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true);
                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                sellItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            } else {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false);
                sellItemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectBuyItem(Item buyItem) {
        if (buyItem == null) {
            return;
        }
        
        selectedItem = buyItem;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.description;
        buyItemValue.text = "Value: " + selectedItem.value + "g";
    }

    public void SelectSellItem(Item sellItem) {
        if (sellItem == null) {
            return;
        }

        selectedItem = sellItem;
        sellItemName.text  = selectedItem.itemName;
        sellItemDescription.text  = selectedItem.description;
        sellItemValue.text  = "Value: " + Mathf.FloorToInt(selectedItem.value * .5f).ToString() + "g";
    }

    public void BuyItem() {
        if (selectedItem == null) {
            return;
        }

        if (GameManager.instance.currentGold >= selectedItem.value) {
            GameManager.instance.currentGold -= selectedItem.value;
            GameManager.instance.AddItem(selectedItem.itemName);
        }

        goldText.text = GameManager.instance.currentGold.ToString() + "g";
    }

    public void SellItem() {
        if (selectedItem == null) {
            return;
        }

        bool foundItem = GameManager.instance.RemoveItem(selectedItem.itemName);
        if (foundItem) {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.value *.5f);
        }   

        goldText.text = GameManager.instance.currentGold.ToString() + "g";
        ShowSellItems();
    }
}
