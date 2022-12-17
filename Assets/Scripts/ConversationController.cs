using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private GameObject _panelDiscution;

    [Header("UIreferences")]
    [SerializeField] private Image _imgPortrait;
    [SerializeField] private TMP_Text _txtTitle;
    [SerializeField] private TMP_Text _txtDiscution;
    [SerializeField] private Transform _transformButtonHolder;
    [SerializeField] private UIButtonDialogueComponent _prefabButton;

    private SODialogue _currentDialogue;
    
    void Start()
    {
        }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDiscutionPanel(SODialogue dialogue) {
        _panelDiscution.SetActive(true);
        LoadSODialogue(dialogue);
    }

    private void LoadSODialogue(SODialogue soDialogue)
    {
        _currentDialogue = soDialogue;
        if( soDialogue.ImgPortrait) _imgPortrait.sprite = _currentDialogue.ImgPortrait;
        _txtTitle.text = _currentDialogue.Title;
        _txtDiscution.text = _currentDialogue.Text;

        for (int i = 0; i < _transformButtonHolder.childCount; i++) {
           Destroy(_transformButtonHolder.GetChild(i).gameObject ,0.01f);
        }

        for (int i = 0; i < _currentDialogue.ChoiceConsequences.Length; i++) {
            ChoiseConsequence choise = _currentDialogue.ChoiceConsequences[i];
            if (!PlayerScript.IsConditionValue(choise.ChoiceCondition)) continue;
            
            UIButtonDialogueComponent bp =Instantiate(_prefabButton, _transformButtonHolder);
            bp.TxtDescription.text = choise.Note;
            bp.OptionIndex = i;
            bp.ConversationController = this;
        }
    }

    public void SubmitChoice(int index)
    {
        if( _currentDialogue.ChoiceConsequences.Length<=index) {
            Debug.LogWarning("Choise Ask Out of range");
            return;
        }
        ChoiseConsequence choice = _currentDialogue.ChoiceConsequences[index];

        if (choice.ChoiceIndex != 0) {
            PlayerScript.Choises[choice.ChoiceIndex] = choice.ChoiceChange;
        }

        if (choice.GazGain != 0) {
            _playerScript.Gaz += choice.GazGain;
        }

        if (choice.IsQuit)
        {
            UIQuitDiscution(choice.RemoveTriggerOnExite);
        }

        if (choice.NextDialogue != null) {
          LoadSODialogue(choice.NextDialogue);  
        }
    }

    public void UIQuitDiscution(bool triggerIsRemove = false)
    {
        _panelDiscution.SetActive(false);
        _playerScript.QuitDiscution(triggerIsRemove);
    }
}
