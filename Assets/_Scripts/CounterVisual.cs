using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterVisual : MonoBehaviour
{
    [SerializeField] Transform standpointLeft, standpointRight;
    [SerializeField] Transform intermediatePoint;
    [HideInInspector] public CustomerVisual Left, Right;

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
        vis.SetTarget(correctPoint);
    }
}
