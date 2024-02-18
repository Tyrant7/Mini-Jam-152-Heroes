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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public delegate void SandwichEventHandler();
    public event SandwichEventHandler OnSandwichCompleted;

    private GameRuntimeObjects environment;

    [Header("Order Management")]
    [SerializeField] OrderManager orderManager = new OrderManager();
    private const int BaseOrderSize = 7;
    private int bonusOrderSize = 0;

    private Queue<FoodItem> orderQueue = new Queue<FoodItem>();

    [Header("Visuals")]
    [SerializeField] GameObject jumpPrefab;

    [Header("Scoring")]
    [SerializeField] GameObject scorePrefab;
    [SerializeField] AudioClip[] scoreSFX;
    [SerializeField] AudioClip[] missSFX;
    [SerializeField] AudioClip slideSFX;
    [SerializeField] AudioClip timeoutSFX;

    [Header("Days")]
    private const int BaseCustomerCount = 4;
    private const int BaseDayLength = 90;
    private const int BaseRent = 20;
    private const int RentIncrease = 10;
    private int totalOrders = 0;
    private int totalDayLength = 0;

    private DayStats dailyStats;
    private List<DayStats> pastStats = new List<DayStats>();

    [Header("Upgrades")]
    public int TotalMoney = 0;
    private bool gameStarted = false;
    private bool postGame = false;

    private void Start()
    {
        // Just in case we want to playtest without going to the main menu first
        StartDay();
    }

    private void Update()
    {
        if (gameStarted)
        {
            dailyStats.Time += Time.deltaTime;
            float remainingTime = totalDayLength - dailyStats.Time;
            environment.timeVisual.UpdateVisual(remainingTime);
            if (remainingTime <= 0)
            {
                Timeup();
            } 
        }
        else if (!postGame && SceneLoader.Instance.IsGameScene())
        {
            StartDay();
        }
    }

    #region Day Management

    public void ResetGame()
    {
        TotalMoney = 0;
        pastStats.Clear();
        UpgradeManager.Instance.ResetUpgrades();
    }

    private void StartDay()
    {
        // If this is a menu scene this check will fail and the day will not start -> what we intended
        environment = FindObjectOfType<GameRuntimeObjects>();
        if (environment == null)
        {
            return;
        }

        UpgradeBonuses bonuses = UpgradeManager.Instance.GetUpgradeBonuses();
        totalOrders = BaseCustomerCount + bonuses.CustomerBonus;
        environment.lineup.InitializeCustomers(totalOrders);
        if (totalOrders > 0)
        {
            orderManager.InitializeOrder(false);
            if (totalOrders > 1)
            {
                orderManager.InitializeOrder(true);
            }
        }

        totalDayLength = BaseDayLength + bonuses.DayLength;
        bonusOrderSize = bonuses.SandwichSize;

        dailyStats = new DayStats(0, 0, 0, 0, BaseRent + (RentIncrease * pastStats.Count));
        environment.scoreCounter.UpdateDisplay(0);

        environment.stackingController.gameObject.SetActive(false);
        environment.orderSelection.gameObject.SetActive(true);
        environment.counterVisual.SetVisual(environment.lineup.GrabNext(), false);
        environment.counterVisual.SetVisual(environment.lineup.GrabNext(), true);

        gameStarted = true;
    }

    private void Timeup()
    {
        Debug.Log("Out of time!");

        // Need a little animation here

        AudioManager.PlayAudioClip(timeoutSFX);
        EndDay();
    }

    private void EndDay()
    {
        gameStarted = false;
        postGame = true;

        environment.stackingController.gameObject.SetActive(false);
        environment.orderSelection.gameObject.SetActive(false);

        // Find true accuracy based on completed orders
        dailyStats.Accuracy /= dailyStats.OrdersFulfilled;
        pastStats.Add(dailyStats);

        // Update money for upgrade purchasing
        TotalMoney += dailyStats.Score - dailyStats.Rent;

        StartCoroutine(EndDayAnimation());
    }

    private IEnumerator EndDayAnimation()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.Instance.LoadScene("Day_Menu");
        postGame = false;
    }

    public DayStats GetLastDayStats()
    {
        return pastStats[^1];
    }

    public int GetDayNumber()
    {
        return pastStats.Count;
    }

    public GameStats GetGameStats()
    {
        GameStats stats = new GameStats(pastStats.Count, 0, 0);
        foreach (DayStats stat in pastStats)
        {
            stats.MoneyMade += stat.Score;
            stats.OrdersFulfilled += stat.OrdersFulfilled;
        }
        return stats;
    }

    #endregion

    #region Stacking

    public FoodItem GetNextFoodItem()
    {
        if (orderQueue.TryDequeue(out FoodItem next))
        {
            environment.displayQueue.UpdateDisplay(new List<FoodItem>(orderQueue));
            return next;
        }

        Sandwich sandwich = environment.stackingController.ResetSandwich();
        if (sandwich != null)
        {
            StartCoroutine(NextOrderAnimation(sandwich));
        }
        environment.displayQueue.UpdateDisplay(new List<FoodItem>());
        return null;
    }

    private IEnumerator NextOrderAnimation(Sandwich sandwich)
    {
        environment.stackingController.gameObject.SetActive(false);

        // Wait for the items to settle
        yield return new WaitForSeconds(3f);

        // Track our bread
        Bread topBread = sandwich.Items[^1].Item2.GetComponent<Bread>();
        Bread bottomBread = sandwich.Items[0].Item2.GetComponent<Bread>();

        // Score each item
        int totalScore = 0;
        int maxScore = 0;
        Dictionary<(FoodItem, GameObject), int> scores = new Dictionary<(FoodItem, GameObject), int>();
        foreach (var item in sandwich.Items)
        {
            float currentScore = 0;
            if (item.Item2 == topBread.gameObject)
            {
                // Special case for top bread
                // We'll give points based on how close it was
                float dist = Mathf.Abs(topBread.transform.position.x - bottomBread.transform.position.x);
                currentScore = (int)Mathf.Ceil((1f - dist) * item.Item1.PointValue);
            }
            else
            {
                // Simply check if our item is between
                Vector3 itemPos = item.Item2.transform.position;
                if ((itemPos.y >= bottomBread.rightBorder.position.y ||
                    itemPos.y >= bottomBread.leftBorder.position.y) && 
                    itemPos.x <= bottomBread.rightBorder.position.x && 
                    itemPos.x >= bottomBread.leftBorder.position.x)
                {
                    currentScore += (float)item.Item1.PointValue / 2;
                }
                if ((itemPos.y <= topBread.rightBorder.position.y ||
                    itemPos.y <= topBread.leftBorder.position.y) &&
                    itemPos.x <= topBread.rightBorder.position.x &&
                    itemPos.x >= topBread.leftBorder.position.x)
                {
                    currentScore += (float)item.Item1.PointValue / 2;
                }
            }

            maxScore += item.Item1.PointValue;
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
            // Don't score the bottom bread (last item after reverse)
            if (pair == sandwich.Items[^1])
            {
                break;
            }

            GameObject jump = Instantiate(jumpPrefab);
            pair.Item2.transform.SetParent(jump.transform);
            int score = scores.GetValueOrDefault(pair, 0);
            DisplayScore(pair.Item2.transform.position, score, score > 0 ? Color.green : Color.red, true);
            yield return new WaitForSeconds(0.2f);
            pair.Item2.transform.SetParent(sandwich.Root.transform);
            Destroy(jump);
        }

        // Game starts back up again as soon as sandwich is done counting
        environment.orderSelection.gameObject.SetActive(true);

        Animator anim = sandwich.Root.GetComponent<Animator>();
        anim.Play("Wrap_Up");
        AudioManager.PlayAudioClip(slideSFX, 1.3f);
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Wrap_Up"));
        Destroy(sandwich.Root);

        ParticleSingleton.Instance.SpawnBigParticles(sandwich.Items[0].Item2.transform.position);
        DisplayScore(sandwich.Items[0].Item2.transform.position, totalScore, Color.yellow, false);
        OnSandwichCompleted?.Invoke();

        // Update stats
        dailyStats.Score += totalScore;
        dailyStats.Accuracy += (float)totalScore / maxScore;
        dailyStats.OrdersFulfilled++;

        environment.scoreCounter.UpdateDisplay(dailyStats.Score);

        // Check day end
        if (dailyStats.OrdersFulfilled >= totalOrders)
        {
            EndDay();
        }
    }

    private void DisplayScore(Vector3 position, int score, Color colour, bool playSound)
    {
        GameObject scoreText = Instantiate(scorePrefab, position + new Vector3(0.5f, 0.5f, -1), Quaternion.identity);
        TextMeshProUGUI textComp = scoreText.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = score.ToString() + "$";
        textComp.color = colour;
        if (playSound)
            AudioManager.PlayRoundRobin(score > 0 ? scoreSFX : missSFX, 0.3f);
        Destroy(scoreText, 2);
    }

    #endregion

    #region Selecting

    public void SelectOrder(bool left)
    {
        if ((left && (orderManager.orderLeft == null || orderManager.orderLeft.Length == 0)) ||
            (!left && (orderManager.orderRight == null || orderManager.orderRight.Length == 0)))
        {
            return;
        }

        // Get the next order
        orderQueue = orderManager.CreateOrder(BaseOrderSize + bonusOrderSize, left);
        environment.orderSelection.gameObject.SetActive(false);

        // Get the next customer up there
        environment.registerVisual.TakeCustomer(left ? environment.counterVisual.Left : environment.counterVisual.Right, !left && environment.counterVisual.Left != null);
        CustomerVisual next = environment.lineup.GrabNext();
        if (next != null)
        {
            // Only create an order if we have remaining customers
            environment.counterVisual.SetVisual(next, left);
            orderManager.InitializeOrder(left);
        }
        else
        {
            orderManager.DisableOrder(left);
        }

        // We can start stacking once we have an active order
        environment.stackingController.ResetSandwich();
        environment.stackingController.gameObject.SetActive(true);
    }

    public FoodItem[] GetCurrentOrder(bool left)
    {
        return left ? orderManager.orderLeft : orderManager.orderRight;
    }

    #endregion
}
