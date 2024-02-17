using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSelection : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 1000, layerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.SelectOrder(info.transform.CompareTag("Order Left"));
            }

            // Highlight customer
        }
    }

    private void OnEnable()
    {
        // Excammation points or whatever
    }
}
