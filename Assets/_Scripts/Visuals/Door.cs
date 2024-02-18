using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void Open()
    {
        anim.Play("Open", 0, 0);
    }
}
