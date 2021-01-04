using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class HoneycombData
{
    // stripCount should be an odd number...?
    public static HoneycombData Generate(int stripCount, int maxLength) {
        HoneycombData honey = new HoneycombData();
        honey.strips = new StripData[stripCount];

        for (int s = 0; s < stripCount; ++s) {
            int length = GetStripLength(s, stripCount, maxLength);
            StripData strip = new StripData();
            strip.hexes = new HexData[length];
            honey.strips[s] = strip;

            for (int i = 0; i < length; ++i) {
                HexData hex = new HexData();
                strip.hexes[i] = hex;
            }
        }

        return honey;
    }

    public static int GetStripLength(int stripNumber, int stripCount, int maxLength) {
        int middleStrip = stripCount / 2;
        return maxLength - Mathf.Abs(middleStrip - stripNumber);
    }

    public StripData[] strips;
    public PlayerData player = new PlayerData(); // Not needed at stage start.

    public SerializedHoneycombData Serialize() {
        IFormatter formatter = new BinaryFormatter();  
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, this);
        SerializedHoneycombData serializedHoneycomb = new SerializedHoneycombData();
        serializedHoneycomb.bytes = stream.ToArray();
        stream.Close();
        return serializedHoneycomb;
    }

    public int GetStripLength(int stripNumber) {
        int middleStrip = this.strips.Length / 2;
        return HoneycombData.GetStripLength(stripNumber, this.strips.Length, this.strips[middleStrip].hexes.Length);
    }
}
