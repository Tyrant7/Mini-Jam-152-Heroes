using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lineup : MonoBehaviour
{
    [SerializeField] GameObject[] customerPrefabs;
    [SerializeField] Transform[] standpoints;

    Queue<CustomerVisual> customers = new Queue<CustomerVisual>();

    public void InitializeCustomers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject customerG = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)]);
            Transform standpoint = standpoints[Mathf.Clamp(i, 0, standpoints.Length - 1)];
            customerG.transform.position = new Vector3(standpoint.position.x, customerG.transform.position.y, standpoint.position.z);
            customerG.transform.rotation = Quaternion.Euler(customerG.transform.rotation.x, standpoint.transform.eulerAngles.y, customerG.transform.rotation.z);
            customers.Enqueue(customerG.GetComponent<CustomerVisual>());
            customers.Peek().SetTarget(standpoint);
        }
        UpdatePositions();
    }

    public CustomerVisual GrabNext()
    {
        if (customers.Count == 0)
        {
            return null;
        }
        CustomerVisual cv = customers.Dequeue();
        UpdatePositions();
        return cv;
    }

    private void UpdatePositions()
    {
        CustomerVisual[] cvs = customers.ToArray();
        for (int i = 0; i < customers.Count; i++)
        {
            // If we have more customers than standpoints, they'll bunch up on the last one
            int standPointIndex = Mathf.Clamp(i, 0, standpoints.Length - 1);
            cvs[i].SetTarget(standpoints[standPointIndex]);
        }
    }
}
