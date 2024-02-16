using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderManager
{
    [SerializeField] FoodItem bottomBread;
    [SerializeField] FoodItem topBread;
    [SerializeField] FoodItem[] foodItems;
    [SerializeField] int maxSequence = 3;

    public Queue<FoodItem> GenerateOrder(int length)
    {
        List<FoodItem> allItems = new List<FoodItem>(foodItems);
        FoodItem item1 = allItems[Random.Range(0, allItems.Count)];
        allItems.Remove(item1);
        FoodItem item2 = allItems[Random.Range(0, allItems.Count)];
        return GenerateQueue(item1, item2, length, maxSequence);
    }

    private Queue<FoodItem> GenerateQueue(FoodItem item1, FoodItem item2, int length, int maxSequence)
    {
        FoodItem[] foods = new FoodItem[length];
        foods[0] = bottomBread;
        for (int i = 1; i < length - 1; i++)
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
        foods[length - 1] = topBread;
        return new Queue<FoodItem>(foods);
    }
}
