using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stacking : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private GameObject next;
    private GameObject previewObject;

    private void Update()
    {
        if (next == null)
        {
            SetNext();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 1000, layerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(next, info.point, Quaternion.identity);
                SetNext();
            }

            previewObject.transform.position = info.point;
        }
    }

    private void SetNext()
    {
        FoodItem item = GameManager.Instance.GetNextFoodItem();
        next = item.Prefab;
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = Instantiate(next, transform);

        // Disable all physics on the preview object
        Rigidbody[] rbs = previewObject.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
        foreach (Rigidbody rb in rbs)
        {
            Destroy(rb);
        }
        foreach (Collider col in colliders)
        {
            Destroy(col);
        }
    }
}
