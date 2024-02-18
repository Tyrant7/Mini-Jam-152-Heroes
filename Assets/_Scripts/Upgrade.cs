using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "New Upgrade")]
public class Upgrade : ScriptableObject
{
    public string Name;
    public string Description;
    public int Cost;
    public Sprite Icon;

    public FoodItem Unlocks;
    public int CustomerBonus;
    public int DayLengthBonus;
    public int SandwichSizeBonus;
}