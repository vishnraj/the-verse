using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour {
    public bool isPlayer;
    public string[] movesAvailable;

    public string charName;
    public int currentHP, maxHP, currentMP, maxMP, strength, defense, wpnPwr, armrPwr;
    public bool hasDied;

    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite, aliveSprite;

    private bool shouldFade;
    public float fadeSpeed = 1f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (shouldFade) {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.MoveTowards(spriteRenderer.color.a, 0f, fadeSpeed * Time.deltaTime));

            if (spriteRenderer.color.a == 0) {
                gameObject.SetActive(false);
            }
        }
    }

    public void EnemyFade() {
        shouldFade = true;
    }
}
