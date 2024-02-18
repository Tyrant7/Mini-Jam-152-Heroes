using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] slides;
    private int current = 0;

    private void Start()
    {
        image.sprite = slides[0];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextSlide();
        }
    }

    private void NextSlide()
    {
        current++;
        if (current >= slides.Length)
        {
            SceneLoader.Instance.LoadMainMenu();
            return;
        }
        image.sprite = slides[current];
    }
}
