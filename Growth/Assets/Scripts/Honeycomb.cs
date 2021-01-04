using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class.
public class Honeycomb : MonoBehaviour
{
    public static Honeycomb instance = null;

    public Strip stripPrefab;
    public float edgeLength;

    private List<Strip> strips;

    void Awake() {
        Honeycomb.instance = this;

        this.strips = new List<Strip>();
    }

    void Destroy() {
        if (Honeycomb.instance == this) {
            Honeycomb.instance = null;
        }
    }

    // Creates a new honeycomb in the game, initialized with the given data.
    public void InitializeWithData(HoneycombData data) {
        // Destroy all old game objects.
        foreach (Strip strip in this.strips) {
            GameObject.Destroy(strip);
        }
        this.strips.Clear();

        // Compute the honeycomb properties.
        float stripWidth = Util.edgeLengthToStripWidth(this.edgeLength);

        // Compute the position of the first strip.
        // The center of each strip is the middle of the strip.
        // Each strip runs parallel to the y-axis. Their centers are in a line parallel to the x-axis.
        Vector3 nextPosition = this.transform.position;
        nextPosition.x -= (data.strips.Length - 1f) * 0.5f * stripWidth;
        // Create the strips.
        for (int i = 0; i < data.strips.Length; ++i) {
            StripData stripData = data.strips[i];
            Strip strip = Instantiate<Strip>(this.stripPrefab, nextPosition, Quaternion.identity);
            strip.stripNumber = i;
            this.strips.Add(strip);
            strip.InitializeWithData(stripData, this.edgeLength);
            // Set the next hex's position.
            nextPosition.x += stripWidth;
        }
    }

    // Sets the honeycomb's current data. Assumes the data has the same honeycomb size as the current honeycomb.
    // To change the size of the honeycomb, use InitializeWithData instead.
    public void SetData(HoneycombData data) {
        if (this.strips.Count != data.strips.Length) {
            Debug.LogError("Cannot set honeycomb data: data has wrong number of strips.");
            return;
        }
        for (int i = 0; i < this.strips.Count; ++i) {
            StripData stripData = data.strips[i];
            this.strips[i].SetData(stripData);
        }
    }

    // Returns the hex at the given location. If the location is invalid, returns null.
    public Hex GetHex(int stripNumber, int hexNumber) {
        if (stripNumber < 0 || stripNumber >= this.strips.Count) {
            return null;
        }
        return this.strips[stripNumber].GetHex(hexNumber);
    }

    public int GetStripCount() {
        return this.strips.Count;
    }
}
