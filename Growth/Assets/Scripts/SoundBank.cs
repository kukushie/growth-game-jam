using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance = null;

    void Awake() {
        SoundBank.instance = this;
    }

    void Destroy() {
        if (SoundBank.instance == this) {
            SoundBank.instance = null;
        }
    }

    public AudioSource move;
    public AudioSource no;
    public AudioSource undo;

    public void PlayMove() {
        this.move.Play();
    }

    public void PlayNo() {
        this.no.Play();
    }

    public void PlayUndo() {
        this.undo.Play();
    }
}
