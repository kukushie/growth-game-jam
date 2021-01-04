using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Location
{
    public int strip;
    public int hex;

    public Location(int strip, int hex) {
        this.strip = strip;
        this.hex = hex;
    }
}
