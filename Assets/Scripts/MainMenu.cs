using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public string newGameScene;

    public GameObject continueButton;

    public string loadGameScene;

    // Start is called before the first frame update
    void Start() {
        if (PlayerPrefs.HasKey("Current_Scene")) {
            continueButton.SetActive(true);
        } else {
            continueButton.SetActive(false);
        }

        // When quitting game and coming back to the main menu
        // destroy leftover artifacts
        if (PlayerController.instance != null) {
            Destroy(PlayerController.instance.gameObject);
        }
        if (GameManager.instance != null) {
            Destroy(GameManager.instance.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void Continue() {
        SceneManager.LoadScene(loadGameScene);
    }

    public void NewGame() {
        SceneManager.LoadScene(newGameScene);
    }

    public void Exit() {
        Application.Quit();
    }
}
