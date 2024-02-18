using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAllUnlocked : MonoBehaviour
{
    private void Start()
    {
        ActivateAll(false);
    }

    public void ActivateAll(bool withParticles)
    {
        UnlockWithUpgrade[] unlockables = GetComponentsInChildren<UnlockWithUpgrade>(true);
        foreach (UnlockWithUpgrade unlockable in unlockables)
        {
            bool unlocked = UpgradeManager.Instance.HasUnlocked(unlockable.upgradeName);
            unlockable.gameObject.SetActive(unlocked);
            if (unlocked && withParticles)
            {
                // If this object is a group, instead spawn over each child
                if (unlockable.transform.childCount > 0)
                {
                    foreach (Transform child in unlockable.transform)
                    {
                        ParticleSingleton.Instance.SpawnBigParticles(child.position);
                    }
                }
                else
                {
                    ParticleSingleton.Instance.SpawnBigParticles(unlockable.transform.position);
                }
            }
        }
    }
}
