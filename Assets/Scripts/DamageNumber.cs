using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {
    public Text damageText;

    public float lifetime = 1.0f;
    public float moveSpeed = 1.0f;

    public float placementJitter = .5f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Destroy(gameObject, lifetime);
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

    public void SetDamage(int damageAmount) {
        damageText.text = damageAmount + "";
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0);
    }
}
