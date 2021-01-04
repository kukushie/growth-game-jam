using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexData
{
    public const int VACANT_DATA = 0;
    public const int PLAYER_DATA_HEAD = 0;
    public const int PLAYER_DATA_BODY = 1;
    
    public EntityType entityType;
    public int entityData;
}
