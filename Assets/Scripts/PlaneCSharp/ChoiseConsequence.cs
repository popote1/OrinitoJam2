using System;

[Serializable]
public class ChoiseConsequence
{
    public string Note;
    public int ChoiceCondition;
    public SODialogue NextDialogue;
    public float GazGain=0;
    public int ChoiceIndex = 0;
    public bool ChoiceChange;
    public bool IsQuit;
    public bool RemoveTriggerOnExite;
}
