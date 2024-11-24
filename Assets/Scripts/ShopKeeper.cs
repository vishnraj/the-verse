using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour {
    private bool canOpen;

    public string[] ItemsForSale = new string[40];

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (canOpen && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space))) {
            if (!Shop.instance.shopMenu.activeInHierarchy && PlayerController.instance.canMove) {
                Shop.instance.itemsForSale = ItemsForSale;
                Shop.instance.OpenShop();
            } else if (Shop.instance.shopMenu.activeInHierarchy && Input.GetKeyUp(KeyCode.Space)) {
                Shop.instance.CloseShop();
             }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canOpen = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canOpen = false;
        }
    }
}
