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

    public delegate void SandwichEventHandler();
    public event SandwichEventHandler OnSandwichCompleted;

    [Header("Order Management")]
    [SerializeField] OrderManager orderManager = new OrderManager();
    [SerializeField] int orderLength = 5;

    private Queue<FoodItem> orderQueue = new Queue<FoodItem>();

    [Header("Stacking")]
    [SerializeField] Stacking stackingController;
    [SerializeField] OrderSelection orderSelection;

    [Header("Visuals")]
    [SerializeField] DisplayQueue displayQueue;
    [SerializeField] GameObject jumpPrefab;
    [SerializeField] Lineup lineup;
    [SerializeField] CounterVisual counterVisual;
    [SerializeField] RegisterVisual registerVisual;

    [SerializeField] int NPCCount = 4;

    [Header("Scoring")]
    [SerializeField] float topBreadScoreWeight = 20;
    [SerializeField] GameObject scorePrefab;

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        orderManager.InitializeOrders();

        stackingController.gameObject.SetActive(false);
        orderSelection.gameObject.SetActive(true);
        lineup.InitializeCustomers(NPCCount);
        counterVisual.SetVisual(lineup.GrabNext(), false);
        counterVisual.SetVisual(lineup.GrabNext(), true);
    }

    #region Stacking

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
        displayQueue.UpdateDisplay(new List<FoodItem>());
        return null;
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
        int totalScore = 0;
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
                RaycastHit[] upHits = Physics.RaycastAll(item.Item2.transform.position + Vector3.down, Vector3.up, 1000f);
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
            totalScore += intScore;
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
            if (pair == sandwich.Items[^1])
            {
                break;
            }

            GameObject jump = Instantiate(jumpPrefab);
            pair.Item2.transform.SetParent(jump.transform);
            int score = scores.GetValueOrDefault(pair, 0);
            DisplayScore(pair.Item2.transform.position, score, score > 0 ? Color.green : Color.red);
            yield return new WaitForSeconds(0.3f);
            pair.Item2.transform.SetParent(sandwich.Root.transform);
            Destroy(jump);
        }

        // Game starts back up again as soon as sandwich is done counting
        orderSelection.gameObject.SetActive(true);

        Animator anim = sandwich.Root.GetComponent<Animator>();
        anim.Play("Wrap_Up");
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Wrap_Up"));
        Destroy(sandwich.Root);

        ParticleSingleton.Instance.SpawnBigParticles(sandwich.Items[0].Item2.transform.position);
        DisplayScore(sandwich.Items[0].Item2.transform.position, totalScore, Color.yellow);
        OnSandwichCompleted?.Invoke();
    }

    private void DisplayScore(Vector3 position, int score, Color colour)
    {
        GameObject scoreText = Instantiate(scorePrefab, position + new Vector3(0.5f, 0.5f, -1), Quaternion.identity);
        TextMeshProUGUI textComp = scoreText.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = score.ToString() + "$";
        textComp.color = colour;
        Destroy(scoreText, 2);
    }

    #endregion

    #region Selecting

    public void SelectOrder(bool left)
    {
        orderQueue = orderManager.CreateOrder(orderLength, left);
        orderSelection.gameObject.SetActive(false);

        // We can start stacking once we have an active order
        stackingController.ResetSandwich();
        stackingController.gameObject.SetActive(true);

        // Get the next customer up there
        registerVisual.TakeCustomer(left ? counterVisual.Left : counterVisual.Right, !left && counterVisual.Left != null);
        counterVisual.SetVisual(lineup.GrabNext(), left);
    }

    public FoodItem[] GetCurrentOrder(bool left)
    {
        return left ? orderManager.orderLeft : orderManager.orderRight;
    }

    #endregion
}
