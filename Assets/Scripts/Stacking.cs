using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stacking : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject topBreadPrefab;
    [SerializeField] GameObject prefab;

    private GameObject next;

    private void Start()
    {
        next = prefab;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                Instantiate(next, info.point, Quaternion.identity);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            next = topBreadPrefab;
        }
    }

    public void SetNext(GameObject next)
    {
        this.next = next;    
    }
}
