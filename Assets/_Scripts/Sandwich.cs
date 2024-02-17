using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandwich
{
    public GameObject Root;
    public List<(FoodItem, GameObject)> Items = new List<(FoodItem, GameObject)>();

    public Sandwich(GameObject sandwichRoot)
    {
        Root = sandwichRoot;
    }
}
