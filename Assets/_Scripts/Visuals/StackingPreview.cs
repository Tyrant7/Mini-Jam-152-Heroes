using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackingPreview : MonoBehaviour
{
    [SerializeField] Animator anim;
    private bool hidden = false;

    public void Show()
    {
        if (!hidden)
        {
            return;
        }
        hidden = false;
        anim.Play("Show");
    }

    public void Hide()
    {
        if (hidden)
        {
            return;
        }
        hidden = true;
        anim.Play("Hide");
    }
}
