using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Update() {
        if (Input.GetButtonDown("Quit")) {
            Application.Quit();
        }
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void PlayStage(TextAsset xmlAsset) {
        Stage.selectedStageData = StageData.DeserializeFromXML(xmlAsset.text);
        SceneManager.LoadScene("StageScene", LoadSceneMode.Single);
    }
}
