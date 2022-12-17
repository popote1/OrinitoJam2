using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private GameObject _panelDiscution;
    void Start()
    {
        }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDiscutionPanel()
    {
        _panelDiscution.SetActive(true);
    }

    public void UIQuitDiscution()
    {
        _panelDiscution.SetActive(false);
        _playerScript.QuitDiscution();
    }
}
