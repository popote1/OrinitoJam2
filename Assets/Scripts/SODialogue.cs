using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "SO/SODialogue")]
public class SODialogue : ScriptableObject {
    public Sprite ImgPortrait;
    public string Title;
    [TextArea] public string Text;
    [SerializeField]public ChoiseConsequence[] ChoiceConsequences;
}
