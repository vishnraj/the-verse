using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour {
    public string areaToLoad;
    public string areaTransitionName;
    public AreaEntrance theEntrance;
    public float waitToLaod = 1f;
    public bool shouldLoadAfterFade;

    // Start is called before the first frame update
    void Start() {
        theEntrance.transitionName = areaTransitionName;
    }

    // Update is called once per frame
    void Update() {
        if (shouldLoadAfterFade) {
            waitToLaod -= Time.deltaTime;
            if (waitToLaod <= 0) {
                shouldLoadAfterFade = false;
                SceneManager.LoadScene(areaToLoad);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            shouldLoadAfterFade = true;
            GameManager.instance.fadingBetweenAreas = true;
            UIFade.instance.FadeToBlack();
            PlayerController.instance.areaTransitionName = areaTransitionName;
        }
    }
}
