using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour {
  public GameObject UIScreen;
  public GameObject player;
  public GameObject gameMan;
  public GameObject audioManager;
  public GameObject battleManager;

  // Start is called before the first frame update
  void Start() {
    if (UIFade.instance == null) {
      UIFade.instance = Instantiate(UIScreen).GetComponent<UIFade>();
    }
    if (PlayerController.instance == null) {
      PlayerController clone = Instantiate(player).GetComponent<PlayerController>();
      PlayerController.instance = clone;
    }
    if (GameManager.instance == null) {
      // deactivate children of AreaExit, which are basically
      // the AreaEntrances, which rely on GameManager for some state
      // information - we can activate them again once the GameManager
      // has been created and initialized
      AreaExit[] areaExits = FindObjectsOfType<AreaExit>();
      foreach (AreaExit area in areaExits) {
          for (int i = 0; i < area.transform.childCount; i++) {
              Transform child = area.transform.GetChild(i);
              child.gameObject.SetActive(false);
          }
      }

      Instantiate(gameMan);
    }
    if (AudioManager.instance == null) {
      Instantiate(audioManager);
    }
    if (BattleManager.instance == null) {
      Instantiate(battleManager);
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
