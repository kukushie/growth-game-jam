using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton.
public class PrefabBank : MonoBehaviour
{
    public static PrefabBank instance = null;

    public GameObject greenHex;
    public GameObject playerHead;
    public GameObject playerBody;

    public GameObject redHex;

    public GameObject yellowHex;
    public GameObject plusOne;
    public GameObject plusTwo;
    public GameObject plusThree;
    public GameObject plusSix;

    public GameObject blueHex;
    public GameObject minusOne;
    public GameObject minusTwo;
    public GameObject minusThree;
    public GameObject minusSix;

    void Awake() {
        PrefabBank.instance = this;
    }

    void Destroy() {
        if (PrefabBank.instance == this) {
            PrefabBank.instance = null;
        }
    }
}
