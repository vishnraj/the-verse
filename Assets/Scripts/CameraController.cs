using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour {
    public Transform target;
    public Tilemap tileMap;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;

    // Start is called before the first frame update
    void Start() {
        target = PlayerController.instance.transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimit = tileMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRightLimit = tileMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, -0f);

        PlayerController.instance.SetBounds(tileMap.localBounds.min, tileMap.localBounds.max);
    }

    // Update is called once per frame
    void LateUpdate() {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Clamp the camera to the tilemap bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);

        if (!musicStarted) {
            musicStarted = true;
            AudioManager.instance.PlayBGM(musicToPlay);
        }
    }
}
