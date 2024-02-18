using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWithUpgrade : MonoBehaviour
{
    [SerializeField] string upgradeName;

    private void Start()
    {
        if (!UpgradeManager.Instance.HasUnlocked(upgradeName))
        {
            gameObject.SetActive(false);
        }
    }
}
