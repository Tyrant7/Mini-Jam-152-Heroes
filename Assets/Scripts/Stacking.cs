using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stacking : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private FoodItem next;
    private GameObject previewObject;

    private List<(FoodItem, GameObject)> stackedObjects = new List<(FoodItem, GameObject)>();

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
                stackedObjects.Add((next, Instantiate(next.Prefab, info.point, Quaternion.identity)));
                SetNext();
            }

            previewObject.transform.position = info.point;
        }
    }

    private void SetNext()
    {
        next = GameManager.Instance.GetNextFoodItem();
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = Instantiate(next.Prefab, transform);

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

    public List<(FoodItem, GameObject)> ResetStack()
    {
        List<(FoodItem, GameObject)> cache = new List<(FoodItem, GameObject)>(stackedObjects);
        stackedObjects.Clear();
        return cache;
    }
}
