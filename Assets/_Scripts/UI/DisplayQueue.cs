using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQueue : MonoBehaviour
{
    [SerializeField] GameObject boxPrefab;
    [SerializeField] RectTransform boxParent;
    private List<GameObject> currentBoxes = new List<GameObject>();

    public void UpdateDisplay(List<FoodItem> items)
    {
        foreach (GameObject box in currentBoxes)
        {
            Destroy(box);
        }
        currentBoxes.Clear();

        foreach (FoodItem item in items)
        {
            GameObject newBox = Instantiate(boxPrefab, boxParent);
            newBox.transform.SetAsLastSibling();
            newBox.GetComponent<Image>().sprite = item.Icon;
            currentBoxes.Add(newBox);
        }
    }
}
