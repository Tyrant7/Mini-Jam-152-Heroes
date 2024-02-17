using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterVisual : MonoBehaviour
{
    [SerializeField] Transform registerPoint;
    [SerializeField] Transform intermediatePoint;
    private CustomerVisual currentVisual;

    private void OnEnable()
    {
        GameManager.Instance.OnSandwichCompleted += CleanupRegister;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSandwichCompleted -= CleanupRegister;
    }

    public void TakeCustomer(CustomerVisual vis, bool useIntermediatePoint)
    {
        if (vis == null)
        {
            return;
        }
        StartCoroutine(TakeCustomerOnceReady(vis, useIntermediatePoint));
    }

    private IEnumerator TakeCustomerOnceReady(CustomerVisual vis, bool useIntermediate) 
    {
        yield return new WaitUntil(() => currentVisual == null);
        currentVisual = vis;
        vis.SetTarget(useIntermediate ? intermediatePoint : registerPoint);
        if (useIntermediate)
        {
            StartCoroutine(UpdatePoint());
        }
    }

    private IEnumerator UpdatePoint()
    {
        yield return new WaitUntil(() => currentVisual.IsClose());
        currentVisual.SetTarget(registerPoint);
    }

    private void CleanupRegister()
    {
        if (currentVisual == null)
        {
            return;
        }
        ParticleSingleton.Instance.SpawnBigParticles(currentVisual.gameObject.transform.position);
        Destroy(currentVisual.gameObject);
    }
}
