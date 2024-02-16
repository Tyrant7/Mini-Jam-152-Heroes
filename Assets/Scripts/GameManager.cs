using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] GameObject jumpPrefab;

    [Header("Scoring")]
    [SerializeField] float topBreadScoreWeight = 20;
    [SerializeField] GameObject scorePrefab;

    public FoodItem GetNextFoodItem()
    {
        if (orderQueue.TryDequeue(out FoodItem next))
        {
            var list = new List<FoodItem>(orderQueue);
            list.Add(next);
            displayQueue.UpdateDisplay(list);
            return next;
        }

        Sandwich sandwich = stackingController.ResetSandwich();
        if (sandwich != null)
        {
            StartCoroutine(NextOrderAnimation(sandwich));
        }

        orderQueue = orderManager.GenerateOrder(orderLength);
        displayQueue.UpdateDisplay(new List<FoodItem>(orderQueue));
        return orderQueue.Dequeue();
    }

    private IEnumerator NextOrderAnimation(Sandwich sandwich)
    {
        stackingController.gameObject.SetActive(false);

        // Wait for the items to settle
        yield return new WaitForSeconds(3f);

        // Track our bread
        GameObject topBread = sandwich.Items[^1].Item2;
        GameObject bottomBread = sandwich.Items[0].Item2;

        // Score each item
        int score = 0;
        Dictionary<(FoodItem, GameObject), int> scores = new Dictionary<(FoodItem, GameObject), int>();
        foreach (var item in sandwich.Items)
        {
            float currentScore = 0;
            if (item.Item2 == topBread)
            {
                // Special case for top bread
                // We'll give points based on how close it was
                float dist = Mathf.Abs(topBread.transform.position.x - bottomBread.transform.position.x);
                currentScore = (int)Mathf.Ceil((1.5f - dist) * topBreadScoreWeight);
            }
            else
            {
                RaycastHit[] downHits = Physics.RaycastAll(item.Item2.transform.position + Vector3.up, Vector3.down, 1000f);
                foreach (RaycastHit hit in downHits)
                {
                    // Half of total points for each bread we're inline with
                    if (hit.transform.CompareTag("Bottom Bread"))
                    {
                        currentScore += item.Item1.PointValue / 2;
                    }
                }
                RaycastHit[] upHits = Physics.RaycastAll(item.Item2.transform.position + Vector3.down, Vector3.down, 1000f);
                foreach (RaycastHit hit in upHits)
                {
                    // Half of total points for each bread we're inline with
                    if (hit.transform.CompareTag("Top Bread"))
                    {
                        currentScore += item.Item1.PointValue / 2;
                    }
                }
            }

            int intScore = (int)Mathf.Ceil(currentScore);
            score += intScore;
            scores.Add(item, intScore);
        }

        // Disable all physics
        Rigidbody[] rbs = sandwich.Root.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = sandwich.Root.GetComponentsInChildren<Collider>();
        foreach (Rigidbody rb in rbs)
        {
            Destroy(rb);
        }
        foreach (Collider col in colliders)
        {
            Destroy(col);
        }

        // Make each ingredient jump and display the score we got for it
        sandwich.Items.Reverse();
        foreach (var pair in sandwich.Items)
        {
            GameObject jump = Instantiate(jumpPrefab);
            pair.Item2.transform.SetParent(jump.transform);
            DisplayScore(pair.Item2, scores.GetValueOrDefault(pair, 0));
            yield return new WaitForSeconds(0.35f);
            pair.Item2.transform.SetParent(sandwich.Root.transform);
            Destroy(jump);
        }

        yield return new WaitForSeconds(1f);

        Animator anim = sandwich.Root.GetComponent<Animator>();
        anim.Play("Wrap_Up");
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Wrap_Up"));
        Destroy(sandwich.Root);

        ParticleSingleton.Instance.SpawnBigParticles(sandwich.Items[0].Item2.transform.position);

        Debug.Log("Finished!");

        stackingController.gameObject.SetActive(true);
    }

    private void DisplayScore(GameObject itemObject, int score)
    {
        GameObject scoreText = Instantiate(scorePrefab, itemObject.transform.position + new Vector3(0.5f, 0.5f, -1), Quaternion.identity);
        TextMeshProUGUI textComp = scoreText.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = score.ToString() + "$";
        textComp.color = score > 0 ? Color.green : Color.red;
        Destroy(scoreText, 2);
    }
}
