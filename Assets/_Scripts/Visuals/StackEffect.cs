using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackEffect : MonoBehaviour
{
    [SerializeField] AudioClip[] collisionSFX;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSingleton.Instance.SpawnStackParticles(collision.GetContact(0).point);
        AudioManager.PlayRoundRobin(collisionSFX);
    }
}
