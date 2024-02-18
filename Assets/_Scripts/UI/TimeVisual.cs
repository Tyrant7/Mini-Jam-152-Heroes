using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeVisual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Animator anim;
    [SerializeField] AudioClip tickSFX;

    public void UpdateVisual(float seconds)
    {
        string text = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
        if (text != timerText.text)
        {
            anim.Play("Small_Bump");
            AudioManager.PlayAudioClip(tickSFX, 0.6f);
        }
        timerText.text = text;
    }
}
