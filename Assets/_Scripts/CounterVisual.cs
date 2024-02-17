using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterVisual : MonoBehaviour
{
    [Header("Order hints")]
    [SerializeField] OrderHint hintLeft;
    [SerializeField] OrderHint hintRight;

    [Header("Customers")]
    [SerializeField] Transform standpointLeft, standpointRight;
    [SerializeField] Transform intermediatePoint;
    [HideInInspector] public CustomerVisual Left, Right;

    private void Update()
    {
        // Show hints for orders when customers get close to the counter
        if (Left != null && Left.IsClose() && Left.GetTarget() == standpointLeft)
        {
            FoodItem[] order = GameManager.Instance.GetCurrentOrder(true);
            hintLeft.Show(order[0].Icon, order[1].Icon);
        }
        else
        {
            hintLeft.Hide();
        }

        if (Right != null && Right.IsClose() && Right.GetTarget() == standpointRight)
        {
            FoodItem[] order = GameManager.Instance.GetCurrentOrder(false);
            hintRight.Show(order[0].Icon, order[1].Icon);
        }
        else
        {
            hintRight.Hide();
        }
    }

    public void SetVisual(CustomerVisual vis, bool left)
    {
        if (vis == null)
        {
            return;
        }

        if (left)
        {
            Left = vis;
            vis.SetTarget(intermediatePoint);
            StartCoroutine(SetCorrectPoint(vis, standpointLeft));
            return;
        }
        Right = vis;
        vis.SetTarget(standpointRight);
    }

    private IEnumerator SetCorrectPoint(CustomerVisual vis, Transform correctPoint)
    {
        yield return new WaitUntil(() => vis.IsClose());

        // Make sure we still have control over this customer
        if (vis.GetTarget() == intermediatePoint)
        {
            vis.SetTarget(correctPoint);
        }
    }
}
