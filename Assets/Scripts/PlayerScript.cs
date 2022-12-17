using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    
    [SerializeField] private CarController _carController;
    [SerializeField] private HUDManager _hudManager;
    [SerializeField] private ConversationController _conversationController;
    
    
    
    [Header("Parameters")]
    [SerializeField] private TriggerZone _currentTriggerZone;
    [SerializeField] private float _maxSpeedToInteract = 2;
    [Header("GazControl")] 
    [SerializeField] private float _maxGaz=50;
    [SerializeField] private float _gazDecrease = 1;
    
    

    public static bool[] Choises = new bool[255];

    [field: SerializeField]
    public float Gaz
    {
        get => _gaz;
        set {
            _gaz=value; 
            _hudManager.SetGaz(_gaz , _maxGaz);
        }
    }

    private bool _isInDiscution;
    private float _gaz;

    public static bool IsConditionValue(int index) {
        if (index == 0) return true;
        return Choises[index];
    }
    void Start()
    {
        _gaz = _maxGaz;
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
                _conversationController.OpenDiscutionPanel(_currentTriggerZone.Dialogue);
            }
        }

        if (_carController.IsAccelerate) {
            Gaz -= _gazDecrease * Time.deltaTime;
        }
    }

    public void QuitDiscution(bool triggerIsRemove = false) {
        _carController.CanControl = true;
        _isInDiscution = false;
        if (triggerIsRemove) {
            _currentTriggerZone.gameObject.SetActive(false);
            _currentTriggerZone = null;
        }
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
