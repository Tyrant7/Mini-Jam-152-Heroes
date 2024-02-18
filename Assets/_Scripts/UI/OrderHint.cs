using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderHint : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Image image1, image2;
    private bool hidden = true;

    [SerializeField] AudioClip[] showSFX;
    [SerializeField] AudioClip[] hideSFX;

    public void Show(Sprite left, Sprite right)
    {
        if (!hidden)
        {
            return;
        }
        hidden = false;

        anim.Play("Show");
        AudioManager.PlayRoundRobin(showSFX, 1.7f);
        image1.sprite = left;
        image2.sprite = right;
    }

    public void Hide()
    {
        if (hidden)
        {
            return;
        }
        hidden = true;

        anim.Play("Hide");
        AudioManager.PlayRoundRobin(hideSFX, 1.7f);
    }
}
