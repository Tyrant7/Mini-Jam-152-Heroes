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

    [HideInInspector] public FoodItem[] orderLeft = null;
    [HideInInspector] public FoodItem[] orderRight = null;

    public void InitializeOrder(bool left)
    {
        if (left)
        {
            orderLeft = GetRandomFoodPair();
            return;
        }
        orderRight = GetRandomFoodPair();
    }

    public void DisableOrder(bool left)
    {
        if (left)
        {
            orderLeft = null;
            return;
        }
        orderRight = null;
    } 

    private FoodItem[] GetRandomFoodPair()
    {
        FoodItem[] items = new FoodItem[2] { foodItems[Random.Range(0, foodItems.Length)], null };
        List<FoodItem> allItems = new List<FoodItem>(foodItems);
        allItems.Remove(items[0]);
        items[1] = allItems[Random.Range(0, allItems.Count)];
        return items;
    }

    public Queue<FoodItem> CreateOrder(int length, bool left)
    {
        FoodItem[] cache = left ? orderLeft : orderRight;
        if (left)
        {
            orderLeft = GetRandomFoodPair();
        }
        else
        {
            orderRight = GetRandomFoodPair();
        }
        return GenerateQueue(cache[0], cache[1], length);
    }

    private Queue<FoodItem> GenerateQueue(FoodItem item1, FoodItem item2, int length)
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
