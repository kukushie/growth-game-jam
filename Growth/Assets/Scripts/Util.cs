using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public const float SQRT_3 = 1.7320508075688772935274463415059f;

    // Converts the length of an edge of a regular hexagon into the distance between opposite edges of the hexagon.
    public static float edgeLengthToHexHeight(float edgeLength) {
        return edgeLength * SQRT_3;
    }

    // Converts the length of an edge of a regular hexagon into the width of a strip of hexagons.
    // The strip width is also the distance between the centers of two adjacent strips.
    public static float edgeLengthToStripWidth(float edgeLength) {
        return edgeLength * 1.5f;
    }

    // Returns the location that is in the given direction from the given location.
    // The returned location may be invalid.
    public static Location GetLocationInDirection(int oldStrip, int oldHex, Direction dir) {
        int middleStrip = Honeycomb.instance.GetStripCount() / 2;
        int newStrip;
        int newHex;
        switch (dir) {
            case Direction.UpLeft:
                newStrip = oldStrip - 1;
                if (oldStrip <= middleStrip) {
                    newHex = oldHex;
                } else {
                    newHex = oldHex + 1;
                }
                break;

            case Direction.Up:
                newStrip = oldStrip;
                newHex = oldHex + 1;
                break;

            case Direction.UpRight:
                newStrip = oldStrip + 1;
                if (oldStrip >= middleStrip) {
                    newHex = oldHex;
                } else {
                    newHex = oldHex + 1;
                }
                break;

            case Direction.DownLeft:
                newStrip = oldStrip - 1;
                if (oldStrip <= middleStrip) {
                    newHex = oldHex - 1;
                } else {
                    newHex = oldHex;
                }
                break;

            case Direction.Down:
                newStrip = oldStrip;
                newHex = oldHex - 1;
                break;

            case Direction.DownRight:
                newStrip = oldStrip + 1;
                if (oldStrip >= middleStrip) {
                    newHex = oldHex - 1;
                } else {
                    newHex = oldHex;
                }
                break;

            default:
                Debug.LogError("unknown Direction");
                newStrip = oldStrip;
                newHex = oldHex;
                break;
        }
        return new Location(newStrip, newHex);
    }
}
