using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int stripNumber;
    public int hexNumber;

    private List<GameObject> children;

    // Make these private so we don't accidentally modify them, e.g. when we intend to modify HexData instead.
    private EntityType entityType;
    private int entityData;

    void Awake() {
        this.children = new List<GameObject>();
    }

    public void InitializeWithData(HexData data) {
        this.SetData(data);
    }

    public void SetData(HexData data) {
        // Destroy existing entity.
        foreach (GameObject child in this.children) {
            Destroy(child);
        }
        this.children.Clear();

        this.entityType = data.entityType;
        this.entityData = data.entityData;

        switch (data.entityType) {
            case EntityType.Vacant:
                this.children.Add(Instantiate(PrefabBank.instance.greenHex, this.transform.position, Quaternion.identity));
                break;

            case EntityType.Obstacle:
                this.children.Add(Instantiate(PrefabBank.instance.redHex, this.transform.position, Quaternion.identity));
                break;

            case EntityType.Player:
                this.InstantiatePlayer(data.entityData);
                break;

            case EntityType.Item:
                this.InstantiateItem(data.entityData);
                break;
        }
    }

    public EntityType GetEntityType() {
        return this.entityType;
    }

    public int GetEntityData() {
        return this.entityData;
    }

    // Returns the location that is in the given direction from this hex's location.
    // The returned location may be invalid.
    public Location GetLocationInDirection(Direction dir) {
        return Util.GetLocationInDirection(this.stripNumber, this.hexNumber, dir);
    }

    private void InstantiatePlayer(int data) {
        this.children.Add(Instantiate(PrefabBank.instance.greenHex, this.transform.position, Quaternion.identity));
        if (data == HexData.PLAYER_DATA_HEAD) {
            this.children.Add(Instantiate(PrefabBank.instance.playerHead, this.transform.position, Quaternion.identity));
            Player.instance.SetHead(this);
        } else if (data == HexData.PLAYER_DATA_BODY) {
            this.children.Add(Instantiate(PrefabBank.instance.playerBody, this.transform.position, Quaternion.identity));
            Player.instance.AddBody(this);
        }
    }

    private void InstantiateItem(int data) {
        if (data > 0) {
            this.children.Add(Instantiate(PrefabBank.instance.yellowHex, this.transform.position, Quaternion.identity));
            switch (data) {
                case 1:
                    this.children.Add(Instantiate(PrefabBank.instance.plusOne, this.transform.position, Quaternion.identity));
                    break;
                case 2:
                    this.children.Add(Instantiate(PrefabBank.instance.plusTwo, this.transform.position, Quaternion.identity));
                    break;
                case 3:
                    this.children.Add(Instantiate(PrefabBank.instance.plusThree, this.transform.position, Quaternion.identity));
                    break;
                case 6:
                    this.children.Add(Instantiate(PrefabBank.instance.plusSix, this.transform.position, Quaternion.identity));
                    break;
                default:
                    Debug.LogWarning("unknown item data value '" + data + "'");
                    break;
            }
        } else {
            this.children.Add(Instantiate(PrefabBank.instance.blueHex, this.transform.position, Quaternion.identity));
            switch (data) {
                case -1:
                    this.children.Add(Instantiate(PrefabBank.instance.minusOne, this.transform.position, Quaternion.identity));
                    break;
                case -2:
                    this.children.Add(Instantiate(PrefabBank.instance.minusTwo, this.transform.position, Quaternion.identity));
                    break;
                case -3:
                    this.children.Add(Instantiate(PrefabBank.instance.minusThree, this.transform.position, Quaternion.identity));
                    break;
                case -6:
                    this.children.Add(Instantiate(PrefabBank.instance.minusSix, this.transform.position, Quaternion.identity));
                    break;
                default:
                    Debug.LogWarning("unknown item data value '" + data + "'");
                    break;
            }
        }
    }
}
