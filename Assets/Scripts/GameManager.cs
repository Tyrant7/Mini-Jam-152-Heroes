using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [Header("Order Management")]
    [SerializeField] OrderManager orderManager = new OrderManager();
    [SerializeField] int orderLength = 5;

    private Queue<FoodItem> orderQueue = new Queue<FoodItem>();

    [Header("Stacking")]
    [SerializeField] Stacking stackingController;

    public FoodItem GetNextFoodItem()
    {
        if (orderQueue.TryDequeue(out FoodItem next))
        {
            return next;
        }

        var stack = stackingController.ResetStack();
        if (stack.Count > 0)
        {
            StartCoroutine(NextOrderAnimation(stack));
        }

        orderQueue = orderManager.GenerateOrder(orderLength);
        return orderQueue.Dequeue();
    }

    private IEnumerator NextOrderAnimation(List<(FoodItem, GameObject)> stackedFood)
    {
        stackingController.gameObject.SetActive(false);

        // Wait for the items to settle
        yield return new WaitForSeconds(2f);

        // Score each item and zoom them off from top to bottom
        stackedFood.Reverse();
        foreach (var pair in stackedFood)
        {
            pair.Item2.GetComponentInChildren<Rigidbody>().AddExplosionForce(1000, Vector3.zero, 100);
            Debug.Log("Scored: " + pair.Item1.PointValue);
            yield return new WaitForSeconds(0.5f);
        }

        stackingController.gameObject.SetActive(true);
    }
}
