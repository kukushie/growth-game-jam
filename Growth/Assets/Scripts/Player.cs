using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton class.
public class Player : MonoBehaviour
{
    public static Player instance = null;

    public GameObject growRing;
    public GameObject shrinkRing;

    private Hex headHex;
    private List<Hex> bodyHexes;
    private PlayerState state;

    // Target cursor for growing or shrinking.
    private Hex targetHex;

    void Awake() {
        Player.instance = this;

        this.state = PlayerState.Invalid;
        this.headHex = null;
        this.bodyHexes = new List<Hex>();
    }

    void Destroy() {
        if (Player.instance == this) {
            Player.instance = null;
        }
    }

    void Update() {
        if (Input.GetButtonDown("Quit")) {
            SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        } else if (Input.GetButtonDown("Restart")) {
            Stage.instance.Restart();
            SoundBank.instance.PlayUndo();
        } else if (Input.GetButtonDown("Undo")) {
            Stage.instance.Undo();
            SoundBank.instance.PlayUndo();
        } else if (this.state == PlayerState.Move) {
            this.CheckMoveInputs();
        } else {
            this.CheckSizeChangeInputs();
        }
    }

    public int GetSegmentCount() {
        int segmentCount = this.bodyHexes.Count;
        if (this.headHex != null) {
            segmentCount += 1;
        }
        return segmentCount;
    }

    // Clears the data about where the player is.
    public void Clear() {
        this.headHex = null;
        this.bodyHexes.Clear();
    }

    public void SetHead(Hex hex) {
        // If we already have a head for some reason, just make this another body segment.
        if (this.headHex != null) {
            this.AddBody(hex);
            return;
        }
        this.headHex = hex;
        this.UpdateState();
    }

    public void AddBody(Hex hex) {
        this.bodyHexes.Add(hex);
        this.UpdateState();
    }

    private void UpdateState() {
        int segmentCount = this.GetSegmentCount();
        int expectedSegmentCount = Stage.instance.GetExpectedPlayerSegmentCount();

        if (segmentCount < expectedSegmentCount) {
            this.state = PlayerState.Grow;
            this.ShowTargetAt(this.headHex);
        } else if (segmentCount > expectedSegmentCount) {
            this.state = PlayerState.Shrink;
            this.ShowTargetAt(this.headHex);
        } else {
            this.state = PlayerState.Move;
            this.ShowTargetAt(null);
        }
    }

    // Sets the location of the target effect and shows it.
    // If hex is null, the target effect is hidden.
    private void ShowTargetAt(Hex hex) {
        this.targetHex = hex;
        if (hex == null) {
            this.growRing.SetActive(false);
            this.shrinkRing.SetActive(false);
        } else if (this.state == PlayerState.Grow) {
            this.growRing.SetActive(true);
            this.growRing.transform.position = hex.transform.position;
            this.shrinkRing.SetActive(false);
        } else {
            this.growRing.SetActive(false);
            this.shrinkRing.SetActive(true);
            this.shrinkRing.transform.position = hex.transform.position;
        }
    }

    // Checks if the player should move based on input.
    private void CheckMoveInputs() {
        if (Input.GetButtonDown("UpLeft")) {
            this.MovePlayer(Direction.UpLeft);
        } else if (Input.GetButtonDown("Up")) {
            this.MovePlayer(Direction.Up);
        } else if (Input.GetButtonDown("UpRight")) {
            this.MovePlayer(Direction.UpRight);
        } else if (Input.GetButtonDown("DownLeft")) {
            this.MovePlayer(Direction.DownLeft);
        } else if (Input.GetButtonDown("Down")) {
            this.MovePlayer(Direction.Down);
        } else if (Input.GetButtonDown("DownRight")) {
            this.MovePlayer(Direction.DownRight);
        }
    }

    private void MovePlayer(Direction dir) {
        Hex newHeadHex = null;
        List<Hex> newBodyHexes = new List<Hex>();

        // Try to find the new location for every body segment.
        if (this.headHex != null) {
            if (!this.TryMove(this.headHex, dir, out newHeadHex)) {
                // Move is invalid. Do nothing.
                SoundBank.instance.PlayNo();
                return;
            }
        }
        foreach (Hex hex in this.bodyHexes) {
            Hex newHex = null;
            if (!this.TryMove(hex, dir, out newHex)) {
                // Move is invalid. Do nothing.
                SoundBank.instance.PlayNo();
                return;
            }
            newBodyHexes.Add(newHex);
        }

        // The move is valid. Do the movement.
        Stage.instance.MovePlayer(newHeadHex, newBodyHexes);
        SoundBank.instance.PlayMove();
    }

    private bool TryMove(Hex segmentHex, Direction dir, out Hex newHex) {
        // Try to get the hex at the new location.
        Location loc = segmentHex.GetLocationInDirection(dir);
        newHex = Honeycomb.instance.GetHex(loc.strip, loc.hex);
        if (newHex == null) {
            return false;
        }
        // Check if there is an obstacle.
        if (newHex.GetEntityType() == EntityType.Obstacle) {
            return false;
        }
        return true;
    }

    // Checks if the player should grow, shrink, or move target cursor based on input.
    private void CheckSizeChangeInputs() {
        if (this.targetHex == null) {
            // Something is wrong. Just do nothing and wait for player to undo or restart game.
            return;
        }
        if (Input.GetButtonDown("UpLeft")) {
            this.MoveTarget(Direction.UpLeft);
        } else if (Input.GetButtonDown("Up")) {
            this.MoveTarget(Direction.Up);
        } else if (Input.GetButtonDown("UpRight")) {
            this.MoveTarget(Direction.UpRight);
        } else if (Input.GetButtonDown("DownLeft")) {
            this.MoveTarget(Direction.DownLeft);
        } else if (Input.GetButtonDown("Down")) {
            this.MoveTarget(Direction.Down);
        } else if (Input.GetButtonDown("DownRight")) {
            this.MoveTarget(Direction.DownRight);
        } else if (Input.GetButtonDown("Shrink")) {
            this.ShrinkTarget();
        }
    }

    private void MoveTarget(Direction dir) {
        Hex newHex = null;
        if (!this.TryMove(this.targetHex, dir, out newHex)) {
            // Move is invalid. Do nothing.
            SoundBank.instance.PlayNo();
            return;
        }

        EntityType entityType = newHex.GetEntityType();

        if (this.state == PlayerState.Grow) {
            // Check if we can grow into the new hex.
            if (entityType == EntityType.Player) {
                // Show the target in the new hex.
                this.ShowTargetAt(newHex);
            } else {
                // Grow into the new hex.
                Stage.instance.GrowPlayer(newHex);
            }
            SoundBank.instance.PlayMove();
        } else {
            // Check if we can shrink out of the new hex.
            if (entityType == EntityType.Player) {
                // Show the target in the new hex.
                this.ShowTargetAt(newHex);
                SoundBank.instance.PlayMove();
            }
        }
    }

    private void ShrinkTarget() {
        // Verify the target is on a player segment.
        if (this.targetHex.GetEntityType() != EntityType.Player) {
            Debug.LogWarning("shrink target is not a player hex");
            return;
        }
        Stage.instance.ShrinkPlayer(this.targetHex);
        SoundBank.instance.PlayMove();
    }
}
