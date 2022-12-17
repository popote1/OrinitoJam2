using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOMusic", menuName = "SO/SOMusic")]
public class SOMusic :ScriptableObject {
    public string Name;
    public AudioClip AudioClip;
}
