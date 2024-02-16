using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Order
{
    private FoodItem item1;
    private FoodItem item2;

    public Order(FoodItem item1, FoodItem item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }

    public Queue<FoodItem> GenerateQueue(int length, int maxSequence)
    {
        FoodItem[] foods = new FoodItem[length];
        for (int i = 0; i < length; i++)
        {
            FoodItem next = Random.Range(0, 2) == 0 ? item1 : item2;

            // This will ensure that we don't get long sequences of the same type of item
            if (i >= maxSequence)
            {
                bool onlyOneType = true;
                for (int j = i; j > i - maxSequence; j--)
                {
                    if (next != foods[j])
                    {
                        onlyOneType = false;
                        break;
                    }
                }

                if (onlyOneType)
                {
                    next = next == item1 ? item2 : item1;
                }
            }

            foods[i] = next;
        }
        return new Queue<FoodItem>(foods);
    }
}