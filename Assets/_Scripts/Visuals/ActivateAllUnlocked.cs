using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAllUnlocked : MonoBehaviour
{
    private void Start()
    {
        ActivateAll();
    }

    public void ActivateAll()
    {
        UnlockWithUpgrade[] unlockables = GetComponentsInChildren<UnlockWithUpgrade>(true);
        foreach (UnlockWithUpgrade unlockable in unlockables)
        {
            unlockable.gameObject.SetActive(UpgradeManager.Instance.HasUnlocked(unlockable.upgradeName));
        }
    }
}
