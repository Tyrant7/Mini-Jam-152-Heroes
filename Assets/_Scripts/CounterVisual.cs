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
        vis.SetTarget(intermediatePoint);
        if (left)
        {
            Left = vis;
            StartCoroutine(SetCorrectPoint(vis, standpointLeft));
            return;
        }
        Right = vis;
        StartCoroutine(SetCorrectPoint(vis, standpointRight));
    }

    private IEnumerator SetCorrectPoint(CustomerVisual vis, Transform correctPoint)
    {
        yield return new WaitForSeconds(2f);
        vis.SetTarget(correctPoint);
    }
}
