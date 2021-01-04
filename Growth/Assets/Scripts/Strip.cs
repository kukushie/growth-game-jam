using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strip : MonoBehaviour
{
    public Hex hexPrefab;

    public int stripNumber;
    private List<Hex> hexes;

    void Awake() {
        this.hexes = new List<Hex>();
    }

    public void InitializeWithData(StripData data, float edgeLength) {
        float hexHeight = Util.edgeLengthToHexHeight(edgeLength);

        // Compute the position of the first strip.
        // The center of each hex is the middle of the hex.
        // Their centers are in a line parallel to the y-axis.
        Vector3 nextPosition = this.transform.position;
        nextPosition.y -= (data.hexes.Length - 1f) * 0.5f * hexHeight;
        // Create the hexes.
        for (int i = 0; i < data.hexes.Length; ++i) {
            HexData hexData = data.hexes[i];
            Hex hex = Instantiate<Hex>(hexPrefab, nextPosition, Quaternion.identity);
            hex.stripNumber = this.stripNumber;
            hex.hexNumber = i;
            this.hexes.Add(hex);
            hex.InitializeWithData(hexData);
            // Set the next hex's position.
            nextPosition.y += hexHeight;
        }
    }

    public void SetData(StripData data) {
        if (this.hexes.Count != data.hexes.Length) {
            Debug.LogError("Cannot set strip data: data has wrong number of hexes.");
            return;
        }
        for (int i = 0; i < this.hexes.Count; ++i) {
            HexData hexData = data.hexes[i];
            this.hexes[i].SetData(hexData);
        }
    }

    // Returns the hex at the given location. If the location is invalid, returns null.
    public Hex GetHex(int hexNumber) {
        if (hexNumber < 0 || hexNumber >= this.hexes.Count) {
            return null;
        }
        return this.hexes[hexNumber];
    }
}
