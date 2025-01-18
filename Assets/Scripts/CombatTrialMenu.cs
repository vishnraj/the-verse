using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrialMenu : MonoBehaviour {
    public GameObject combatTrialMenu;

    private bool canActivate;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.gameMenuOpen || GameManager.instance.dialogueActive) {
            return;
        }

        if (canActivate && (Input.GetButtonDown("Fire1") || Input.GetKeyUp(KeyCode.Space)) && !combatTrialMenu.activeInHierarchy) {
            OpenCombatTrialMenu();
        } else if (canActivate && Input.GetKeyUp(KeyCode.Space) && combatTrialMenu.activeInHierarchy) {
            CloseCombatTrialMenu();
        }
    }

    public void OpenCombatTrialMenu() {
        combatTrialMenu.SetActive(true);
        GameManager.instance.combatTrialMenuActive = true;
    }

    public void CloseCombatTrialMenu() {
        combatTrialMenu.SetActive(false);
        GameManager.instance.combatTrialMenuActive = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            canActivate = true;
        }
    }
}
