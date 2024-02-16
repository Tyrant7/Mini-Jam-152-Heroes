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

    [Header("Visuals")]
    [SerializeField] DisplayQueue displayQueue;

    [Header("Scoring")]
    [SerializeField] float topBreadScoreWeight = 20;

    public FoodItem GetNextFoodItem()
    {
        if (orderQueue.TryDequeue(out FoodItem next))
        {
            var list = new List<FoodItem>(orderQueue);
            list.Add(next);
            displayQueue.UpdateDisplay(list);
            return next;
        }

        var stack = stackingController.ResetStack();
        if (stack.Count > 0)
        {
            StartCoroutine(NextOrderAnimation(stack));
        }

        orderQueue = orderManager.GenerateOrder(orderLength);
        displayQueue.UpdateDisplay(new List<FoodItem>(orderQueue));
        return orderQueue.Dequeue();
    }

    private IEnumerator NextOrderAnimation(List<(FoodItem, GameObject)> stackedFood)
    {
        stackingController.gameObject.SetActive(false);

        // Wait for the items to settle
        yield return new WaitForSeconds(3f);

        // Track our bread
        GameObject topBread = stackedFood[stackedFood.Count - 1].Item2;
        GameObject bottomBread = stackedFood[0].Item2;

        // Score each item
        int score = 0;
        Dictionary<(FoodItem, GameObject), int> scores = new Dictionary<(FoodItem, GameObject), int>();
        foreach (var item in stackedFood)
        {
            if (item.Item2 == topBread)
            {
                // Special case for top bread
                // We'll give points based on how close it was
                float dist = Mathf.Abs(topBread.transform.position.x - bottomBread.transform.position.x);
                int currentScore = (int)Mathf.Ceil((1.5f - dist) * topBreadScoreWeight);
                score += currentScore;
                scores.Add(item, currentScore);
                continue;
            }

            RaycastHit[] hits = Physics.RaycastAll(item.Item2.transform.position + Vector3.up, Vector3.down, 1000f);
            foreach (RaycastHit hit in hits)
            {
                if (!hit.transform.CompareTag("Bottom Bread"))
                {
                    continue;
                }

                int currentScore = item.Item1.PointValue;
                score += currentScore;
                scores.Add(item, currentScore);
                break;
            }
        }

        // Whisk items away one by one
        stackedFood.Reverse();
        foreach (var pair in stackedFood)
        {
            pair.Item2.GetComponentInChildren<Rigidbody>().AddExplosionForce(1000, Vector3.zero, 100);
            DisplayScore(pair.Item2, scores.GetValueOrDefault(pair, 0));
            yield return new WaitForSeconds(0.5f);
        }

        stackingController.gameObject.SetActive(true);
    }

    private void DisplayScore(GameObject itemObject, int score)
    {
        Debug.Log(itemObject.name + ": " + score);
        // Display some text
    }
}
