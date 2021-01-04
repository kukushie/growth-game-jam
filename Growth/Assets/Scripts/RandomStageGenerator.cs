using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Singleton.
public class RandomStageGenerator : MonoBehaviour
{
    public static RandomStageGenerator instance = null;

    public InputField seedInput;

    public int minStrips = 9;
    public int maxStrips = 9;
    public int minLongestStripLength = 9;
    public int maxLongestStripLength = 9;

    public int[] possibleGrowthPerSegment;

    public float itemChance = 0.5f;
    public float thenObstacleChance = 0.5f;
    public float plusOneChance = 0.5f;
    public float thenPlusTwoChance = 0.5f;
    public float thenPlusThreeChance = 0.5f;
    public float thenPlusSixChance = 0.5f;
    public float thenMinusOneChance = 0.5f;
    public float thenMinusTwoChance = 0.5f;
    public float thenMinusThreeChance = 0.5f;

    void Awake() {
        RandomStageGenerator.instance = this;
    }    

    void Destroy() {
        if (RandomStageGenerator.instance == this) {
            RandomStageGenerator.instance = null;
        }
    }

    public void RandomizeSeed() {
        this.seedInput.text = Random.Range(int.MinValue, int.MaxValue).ToString();
    }

    public void PlayRandomStage() {
        int seed;
        try {
            seed = int.Parse(this.seedInput.text);
        } catch {
            // Ignore error.
            return;
        }
        Stage.selectedStageData = this.GenerateRandomStage(seed);
        // Output the stage XML so we can maybe use it later.
        Debug.Log(Stage.selectedStageData.SerializeToXML());
        SceneManager.LoadScene("StageScene", LoadSceneMode.Single);
    }

    private StageData GenerateRandomStage(int seed) {
        Random.InitState(seed);
        int stripCount = Random.Range((this.minStrips - 1) / 2, (this.maxStrips - 1) / 2) * 2 + 1; // Should be odd?
        int min = Mathf.Max(this.minLongestStripLength, stripCount / 2 + 1);
        int maxStripLength = Random.Range(min, this.maxLongestStripLength + 1);
        HoneycombData honey = HoneycombData.Generate(stripCount, maxStripLength);

        // For each hex, give it random contents.
        foreach (StripData stripData in honey.strips) {
            foreach (HexData data in stripData.hexes) {
                this.RandomizeHex(data);
            }
        }

        // Pick a random starting location.
        // Do this last so it replaces whatever was at the hex's location.
        int strip = Random.Range(0, stripCount);
        int hex = Random.Range(0, honey.GetStripLength(strip));
        HexData hexData = honey.strips[strip].hexes[hex];
        hexData.entityType = EntityType.Player;
        hexData.entityData = HexData.PLAYER_DATA_HEAD;

        StageData stageData = new StageData();
        stageData.name = "Random stage from seed " + seed;
        stageData.growthPerSegment = this.possibleGrowthPerSegment[Random.Range(0, this.possibleGrowthPerSegment.Length)];
        stageData.honeycomb = honey;
        return stageData;
    }

    private void RandomizeHex(HexData data) {
        if (Random.value < this.itemChance) {
            // Make item.
            data.entityType = EntityType.Item;
            if (Random.value < this.plusOneChance) {
                data.entityData = 1;
                return;
            }
            if (Random.value < this.thenPlusTwoChance) {
                data.entityData = 2;
                return;
            }
            if (Random.value < this.thenPlusThreeChance) {
                data.entityData = 3;
                return;
            }
            if (Random.value < this.thenPlusSixChance) {
                data.entityData = 6;
                return;
            }
            if (Random.value < this.thenMinusOneChance) {
                data.entityData = -1;
                return;
            }
            if (Random.value < this.thenMinusTwoChance) {
                data.entityData = -2;
                return;
            }
            if (Random.value < this.thenMinusThreeChance) {
                data.entityData = -3;
                return;
            }
            data.entityData = -6;
            return;
        }
        if (Random.value < this.thenObstacleChance) {
            // Make obstacle.
            data.entityType = EntityType.Obstacle;
            data.entityData = 0;
            return;
        }
        // Make vacant.
        data.entityType = EntityType.Vacant;
        data.entityData = HexData.VACANT_DATA;
    }
}
