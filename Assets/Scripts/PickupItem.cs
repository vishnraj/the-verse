using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour {
    private bool canPickup;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (canPickup && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space)) && PlayerController.instance.canMove) {
            GameManager.instance.SortItems();
            if (GameManager.instance.GetItemDetails(GetComponent<Item>().itemName).isKeyItem) {
                GameManager.instance.AddKeyItem(GetComponent<Item>().itemName);
            } else {
                GameManager.instance.AddItem(GetComponent<Item>().itemName);
            }
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canPickup = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            canPickup = false;
        }
    }
}
