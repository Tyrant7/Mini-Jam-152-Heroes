using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Animator textAnim;

    public void UpdateDisplay(int score)
    {
        scoreText.text = score.ToString() + "$";
        textAnim.Play("Bump");
    }
}
