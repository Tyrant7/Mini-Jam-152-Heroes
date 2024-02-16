using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackEffect : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        ParticleSingleton.Instance.SpawnStackParticles(collision.GetContact(0).point);
    }
}
