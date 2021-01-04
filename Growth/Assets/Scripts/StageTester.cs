using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTester : MonoBehaviour
{
    public TextAsset stageXML;
    public int growthPerSegment = 1;
    public HoneycombData honeycombData;

    // Start is called before the first frame update
    void Start()
    {
        if (Stage.selectedStageData != null) {
            // A stage was selected from the main menu. Don't load the test stage.
            return;
        }

        // Load the test stage.
        StageData data;
        if (this.stageXML == null) {
            data = new StageData();
            data.growthPerSegment = this.growthPerSegment;
            data.honeycomb = this.honeycombData;
        } else {
            data = StageData.DeserializeFromXML(this.stageXML.text);
        }
        Stage.instance.InitializeWithData(data);
    }
}
