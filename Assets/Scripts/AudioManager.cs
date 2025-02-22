using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public AudioSource[] bgm;
    public AudioSource[] sfx;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            if (instance != this) {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {

    }

    public void TestAudio() {
        if (Input.GetKeyDown(KeyCode.T)) {
            PlaySFX(4);
            PlayBGM(3);
        }
    }

    public void PlaySFX(int soundToPlay) {
        if (soundToPlay < sfx.Length) {
            sfx[soundToPlay].Play();
        }
    }

    public void PlayBGM(int musicToPlay) {
        if (bgm[musicToPlay].isPlaying) {
            return;
        }

        StopMusic();

        if (musicToPlay < bgm.Length) {
            bgm[musicToPlay].Play();
        }
    }

    public void StopMusic() {
        for (int i = 0; i < bgm.Length; i++) {
            bgm[i].Stop();
        }
    }
}
