using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    
    [SerializeField] private CarController _carController;
    [SerializeField] private TriggerZone _currentTriggerZone;
    [SerializeField] private ConversationController _conversationController;
    
    
    
    [Header("Parameters")]
    [SerializeField] private float _maxSpeedToInteract = 2;

    private bool _isInDiscution;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTriggerZone) {
            if (_carController.IsSlowerThan(_maxSpeedToInteract)) {
                _currentTriggerZone.ShowInteractable(true);
            }
            else {
                _currentTriggerZone.ShowInteractable(false);
            }

            if (Input.GetKeyDown(KeyCode.Space)&&!_isInDiscution) {
                _carController.CanControl = false;
                _conversationController.OpenDiscutionPanel();
            }
        }
    }

    public void QuitDiscution()
    {
        _carController.CanControl = true;
        _isInDiscution = false;
    }

    public void SetTriggerZone(TriggerZone triggerZone) {
        if (_currentTriggerZone == null) {
            _currentTriggerZone = triggerZone;
        }
    }

    public void ExitTriggerZone(TriggerZone triggerZone) {
        if (_currentTriggerZone == triggerZone) {
            triggerZone.ShowInteractable(false);
            _currentTriggerZone = null;
        }
    }
}
