using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAllUnlocked : MonoBehaviour
{
    private void Start()
    {
        UnlockWithUpgrade[] unlockables = GetComponentsInChildren<UnlockWithUpgrade>(true);
        foreach (UnlockWithUpgrade unlockable in unlockables)
        {
            unlockable.ActivateIfUnlocked();
        }
    }
}
