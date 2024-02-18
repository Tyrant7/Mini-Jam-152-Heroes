using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackEffect : MonoBehaviour
{
    [SerializeField] AudioClip[] collisionSFX;

    const float minSoundInterval = 0.5f;
    float lastSoundTime = -minSoundInterval;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSingleton.Instance.SpawnStackParticles(collision.GetContact(0).point);
        if (Time.time - lastSoundTime >= minSoundInterval)
        {
            lastSoundTime = Time.time;
            AudioManager.PlayRoundRobin(collisionSFX, 0.5f);
        }
    }
}
