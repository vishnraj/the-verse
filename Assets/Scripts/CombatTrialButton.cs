using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatTrialButton : MonoBehaviour {
    public Text buttonText;
    public int buttonValue;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Press() {
        CombatTrialMenuManager.instance.StartTrial(buttonValue);
    }
}
