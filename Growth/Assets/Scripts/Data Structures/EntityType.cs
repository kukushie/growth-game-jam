using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Invalid = -99,
    Vacant = 0,
    Obstacle = 1,
    Player = 2, // Data: 0 = head, 1 = body.
    Item = 3, // Data: number of growth points gained or lost.
    Powerup = 4, // Data: powerup type.
}
