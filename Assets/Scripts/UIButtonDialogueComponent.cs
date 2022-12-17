using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonDialogueComponent : MonoBehaviour
{
    public Button BPOption;
    public TMP_Text TxtDescription;
    public int OptionIndex;

    public ConversationController ConversationController;

    public void PressButton() {
        ConversationController.SubmitChoice(OptionIndex);
    }


}
