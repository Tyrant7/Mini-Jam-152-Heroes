using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Soundtrack", menuName ="New Soundtrack")]
public class Soundtrack : ScriptableObject
{
    public string Name;
    public AudioClip Track;
}
