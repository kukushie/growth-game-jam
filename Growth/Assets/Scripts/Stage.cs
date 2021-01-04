using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Singleton class.
public class Stage : MonoBehaviour
{
    private const string STATUS_TEXT_FORMAT = @"Growth: {0} ({1} more to grow)
Size: {2}
Items Collected: {3}
Moves: {4}";

    public static Stage instance = null;
    public static StageData selectedStageData = null;

    public Text statusText;
    public Text stageNameText;
    public Text stageInstructionsText;

    private int growthPerSegment;
    private HoneycombData currentData;
    private Stack<SerializedHoneycombData> undoStack;

    void Awake() {
        Stage.instance = this;

        this.undoStack = new Stack<SerializedHoneycombData>();
    }

    void Start() {
        if (Stage.selectedStageData != null) {
            // A stage was selected from the main menu. Start the stage immediately.
            this.InitializeWithData(Stage.selectedStageData);
        }
    }

    void Destroy() {
        if (Stage.instance == this) {
            Stage.instance = null;
        }
    }

    public void InitializeWithData(StageData data) {
        this.growthPerSegment = data.growthPerSegment;
        this.currentData = data.honeycomb;

        this.stageNameText.text = data.name;
        this.stageInstructionsText.text = data.instructions;

        // Initialize the player's data.
        this.currentData.player.totalGrowth = 0;
        this.currentData.player.itemsCollected = 0;
        this.currentData.player.powerupsCollected = 0;
        this.currentData.player.totalMoves = 0;
        Player.instance.Clear();

        // Initialize the honeycomb.
        Honeycomb.instance.InitializeWithData(this.currentData);
        this.UpdateUI();
    }

    public int GetExpectedPlayerSegmentCount() {
        return 1 + this.currentData.player.totalGrowth / this.growthPerSegment;
    }

    // Moves the player's head and body to the given hexes.
    // Assumes the movement is valid.
    public void MovePlayer(Hex headHex, List<Hex> bodyHexes) {
        this.PushUndoData();

        // Move the player out of all hexes they currently occupy.
        // When a player leaves a hex, it is always left vacant.
        foreach (StripData stripData in this.currentData.strips) {
            foreach (HexData hexData in stripData.hexes) {
                if (hexData.entityType == EntityType.Player) {
                    this.RemovePlayerFromHexData(hexData);
                }
            }
        }

        // Move player into all new hexes.
        if (headHex != null) {
            this.MovePlayerSegmentOntoHex(headHex, true);
        }
        foreach (Hex hex in bodyHexes) {
            this.MovePlayerSegmentOntoHex(hex, false);
        }

        this.currentData.player.totalMoves += 1;
        this.UpdateGameFromData();
    }

    // Grows the player into the given hex.
    // Assumes the growth is valid.
    public void GrowPlayer(Hex hex) {
        this.PushUndoData();
        this.MovePlayerSegmentOntoHex(hex, false);
        this.UpdateGameFromData();
    }

    // Removes the player from the given hex. If the player's head is removed, a body segment will become their head.
    // Assumes the shrinkage is valid and the player has at least one other segment remaining.
    public void ShrinkPlayer(Hex hex) {
        this.PushUndoData();

        HexData hexData = this.currentData.strips[hex.stripNumber].hexes[hex.hexNumber];

        // Check if we are shrinking the head.
        if (hexData.entityType == EntityType.Player && hexData.entityData == HexData.PLAYER_DATA_HEAD) {
            // Make a body segment the new head.
            this.SetNewPlayerHead();
        }

        // Remove the player from the hex.
        this.RemovePlayerFromHexData(hexData);

        this.UpdateGameFromData();
    }

    public void Undo() {
        if (this.undoStack.Count == 0) {
            // Do nothing.
            return;
        }
        this.currentData = this.undoStack.Pop().Deserialize();
        this.UpdateGameFromData();
    }

    public void Restart() {
        // Restart the stage by returning to the first undo data.
        while (this.undoStack.Count > 1) {
            this.undoStack.Pop();
        }
        this.Undo();
    }

    private void RemovePlayerFromHexData(HexData data) {
        // When a player leaves a hex, it is always left vacant.
        data.entityType = EntityType.Vacant;
        data.entityData = HexData.VACANT_DATA;
    }

    private void MovePlayerSegmentOntoHex(Hex hex, bool isHead) {
        HexData data = this.currentData.strips[hex.stripNumber].hexes[hex.hexNumber];

        // Check if the hex has an item. If so, apply its effects.
        if (data.entityType == EntityType.Item) {
            this.currentData.player.totalGrowth = System.Math.Max(0, this.currentData.player.totalGrowth + data.entityData);
            this.currentData.player.itemsCollected += 1;
        }

        // Turn the hex into a player hex.
        data.entityType = EntityType.Player;
        data.entityData = (isHead ? HexData.PLAYER_DATA_HEAD : HexData.PLAYER_DATA_BODY);
    }

    // Changes the first body segment we come across into the player's head.
    private void SetNewPlayerHead() {
        foreach (StripData stripData in this.currentData.strips) {
            foreach (HexData hexData in stripData.hexes) {
                if (hexData.entityType == EntityType.Player && hexData.entityData == HexData.PLAYER_DATA_BODY) {
                    hexData.entityData = HexData.PLAYER_DATA_HEAD;
                    // Our work here is done.
                    return;
                }
            }
        }
    }

    // Pushes the current game data onto the undo stack.
    private void PushUndoData() {
        this.undoStack.Push(this.currentData.Serialize());
    }

    private void UpdateGameFromData() {
        Player.instance.Clear();
        Honeycomb.instance.SetData(this.currentData);
        this.UpdateUI();
    }

    private void UpdateUI() {
        int growth = this.currentData.player.totalGrowth;
        int nextGrowAt = this.growthPerSegment - (growth % this.growthPerSegment);
        int size = Player.instance.GetSegmentCount();
        int items = this.currentData.player.itemsCollected;
        int moves = this.currentData.player.totalMoves;
        this.statusText.text = string.Format(STATUS_TEXT_FORMAT, growth, nextGrowAt, size, items, moves);
    }
}
