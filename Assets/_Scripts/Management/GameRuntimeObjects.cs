using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameRuntimeObjects : MonoBehaviour
{
    // If I had time, this would be a much more elegant system and
    // everything wouldn't be handled through a weird dependancy chain in the GameManager, but whatever!

    [Header("Stacking")]
    public Stacking stackingController;
    public OrderSelection orderSelection;

    [Header("Visuals")]
    public DisplayQueue displayQueue;

    public Lineup lineup;
    public CounterVisual counterVisual;
    public RegisterVisual registerVisual;

    public ScoreCounter scoreCounter;
    public TimeVisual timeVisual;

    public TextMeshProUGUI scoreText;
}
