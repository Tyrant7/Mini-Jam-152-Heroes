using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public class SoundtrackManager : MonoBehaviour
{
    #region Singleton

    public static SoundtrackManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.root == transform)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] AudioSource source;
    [SerializeField] Animator popupAnim;
    [SerializeField] TextMeshProUGUI popupText;

    [SerializeField] Soundtrack[] soundtracks;

    private Queue<Soundtrack> queue = new Queue<Soundtrack>();

    private void Start()
    {
        StartCoroutine(PlayNextForever());
    }

    private IEnumerator PlayNextForever()
    {
        while (Application.isPlaying)
        {
            // Refresh out queue if empty
            if (queue.Count == 0)
            {
                Soundtrack[] shuffledTracks = soundtracks.OrderBy(_ => Random.Range(int.MinValue, int.MaxValue)).ToArray();
                queue = new Queue<Soundtrack>(shuffledTracks);
            }
            Soundtrack next = queue.Dequeue();
            source.clip = next.Track;
            source.Play();

            // Visuals!
            popupText.text = next.Name;
            popupAnim.Play("Popup");

            // Continue once it's finished
            yield return new WaitForSeconds(next.Track.length);

            // Give the player a few seconds to enjoy the silence
            yield return new WaitForSeconds(5f);
        }
    }

    public void DisableEnableSoundtracks()
    {
        source.volume = source.volume == 0 ? 0.7f : 0;
    }
}
