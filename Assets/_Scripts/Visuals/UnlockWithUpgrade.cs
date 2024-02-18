using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWithUpgrade : MonoBehaviour
{
    [SerializeField] string upgradeName;

    public void ActivateIfUnlocked()
    {
        gameObject.SetActive(UpgradeManager.Instance.HasUnlocked(upgradeName));
    }
}
