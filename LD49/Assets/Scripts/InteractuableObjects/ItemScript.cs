using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Coffe,
    Paper,
    Water,
    Food
}

public class ItemScript : MonoBehaviour
{
    public float weight = 0f;
    public ItemType type;

}
