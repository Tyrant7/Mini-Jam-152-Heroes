using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "New Food Item")]
public class FoodItem : ScriptableObject
{
    public GameObject Prefab;
    public Sprite Icon;
    public int PointValue;
    public bool DontRotate;
}