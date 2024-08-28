using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public int gameOverMusicIndex;
    public string mainMenuScene;
    public string loadGameScene;
    public GameObject eventSystem;

    // Start is called before the first frame update
    void Start() {
        if (AudioManager.instance != null) {
            AudioManager.instance.PlayBGM(2);
        }
        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy) {
            PlayerController.instance.gameObject.SetActive(false);
        }
        if (GameMenu.instance != null && GameMenu.instance.gameObject.activeInHierarchy) {
            GameMenu.instance.gameObject.SetActive(false);
            eventSystem.SetActive(true);
        }
        if (BattleManager.instance != null && BattleManager.instance.gameObject.activeInHierarchy) {
            BattleManager.instance.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if (AudioManager.instance != null) {
            AudioManager.instance.PlayBGM(gameOverMusicIndex);
        }
        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy) {
            PlayerController.instance.gameObject.SetActive(false);
        }
        if (GameMenu.instance != null && GameMenu.instance.gameObject.activeInHierarchy) {
            GameMenu.instance.gameObject.SetActive(false);
            eventSystem.SetActive(true);
        }
        if (BattleManager.instance != null && BattleManager.instance.gameObject.activeInHierarchy) {
            BattleManager.instance.gameObject.SetActive(false);
        }
    }

    public void QuitToMain() {
        if (GameManager.instance != null) {
            Destroy(GameManager.instance.gameObject);
        }
        if (PlayerController.instance != null) {
            Destroy(PlayerController.instance.gameObject);
        }
        if (GameMenu.instance != null) {
            Destroy(GameMenu.instance.gameObject);
        }
        if (BattleManager.instance != null) {
            Destroy(BattleManager.instance.gameObject);
        }
        if (AudioManager.instance != null) {
            Destroy(AudioManager.instance.gameObject);
        }

        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLastSave() {
        if (GameManager.instance != null) {
            Destroy(GameManager.instance.gameObject);
        }
        if (PlayerController.instance != null) {
            Destroy(PlayerController.instance.gameObject);
        }
        if (GameMenu.instance != null) {
            Destroy(GameMenu.instance.gameObject);
        }
        if (BattleManager.instance != null) {
            Destroy(BattleManager.instance.gameObject);
        }

        SceneManager.LoadScene(loadGameScene);
    }
}
