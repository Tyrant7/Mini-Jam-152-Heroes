using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSingleton : MonoBehaviour
{
    #region Singleton

    public static ParticleSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] GameObject stackParticles;
    [SerializeField] GameObject bigDustParticles;

    public void SpawnStackParticles(Vector3 position)
    {
        Instantiate(stackParticles, position, Quaternion.identity, transform);
    }

    public void SpawnBigParticles(Vector3 position)
    {
        Instantiate(bigDustParticles, position, Quaternion.identity, transform);
    }
}
