using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stacking : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private FoodItem next;
    private GameObject previewObject;
    private Quaternion nextRotation;

    private Sandwich sandwich;

    [SerializeField] GameObject previewEffectPrefab;
    [SerializeField] GameObject sandwichRootPrefab;

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
                sandwich.Items.Add((next, Instantiate(next.Prefab, info.point, nextRotation, sandwich.Root.transform)));
                SetNext();
            }

            previewObject.transform.position = info.point;
        }
    }

    private void SetNext()
    {
        next = GameManager.Instance.GetNextFoodItem();
        if (next == null)
        {
            return;
        }

        nextRotation = next.DontRotate ? Quaternion.identity : Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = Instantiate(previewEffectPrefab, Vector3.zero, Quaternion.identity, transform);
        Instantiate(next.Prefab, Vector3.zero, nextRotation, previewObject.transform);

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

    public Sandwich ResetSandwich()
    {
        Sandwich cache = sandwich;
        sandwich = new Sandwich(Instantiate(sandwichRootPrefab, new Vector3(0, 0.78f, 0), Quaternion.identity));
        return cache;
    }
}
