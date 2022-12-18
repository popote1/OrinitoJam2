using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    
    [SerializeField] private CarController _carController;
    [SerializeField] private HUDManager _hudManager;
    [SerializeField] private ConversationController _conversationController;

    public bool IsPlayng;
    
    [Header("Parameters")]
    [SerializeField] private TriggerZone _currentTriggerZone;
    [SerializeField] private float _maxSpeedToInteract = 2;
    [Header("GazControl")] 
    [SerializeField] private float _maxGaz=50;
    [SerializeField] private float _gazDecrease = 1;

    [SerializeField] private SOMusic[] _musics;
    [SerializeField] private AudioSource _asMusic;
    
    
    

    public static bool[] Choises = new bool[255];
    
    public float Gaz
    {
        get => _gaz;
        set {
            _gaz=value;
            _hudManager.SetGaz(_gaz , _maxGaz);

            if (IsPlayng && _gaz <= 0)
            {
                DoGameOver();
            }
        }
    }

    private bool _isInDiscution;
    private float _gaz;
    private int _currentMusic;
    private bool _musicIsPlaying;
    private float _musicTimer;

    public static bool IsConditionValue(int index) {
        if (index == 0) return true;
        return Choises[index];
    }
    void Start() {
        _gaz = _maxGaz;
        if (_musics != null && _musics.Length > 0) {
            _asMusic.clip = _musics[0].AudioClip;
            _currentMusic = 0;
            _hudManager.SetMusicData(_musics[0]);
            _musicIsPlaying = true;
            _asMusic.Play();
        }
    }

    public void SetIsPlaying() {
        IsPlayng = true;
        _carController.CanControl = true;
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

        if (_carController.IsAccelerate&&IsPlayng) {
            Gaz -= _gazDecrease * Time.deltaTime;
        }

        if (_musicIsPlaying) {
            _musicTimer += Time.deltaTime;
            if (_musicTimer >= _musics[_currentMusic].AudioClip.length) {
                _musicTimer = 0;
                ChangeSong(+1);
            }
        }
    }

    private void DoGameOver() {
        _carController.CanControl = false;
        _carController.AsMotorSound.Stop();
        _hudManager.OpenGameOverPanel();
        IsPlayng = false;
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

    private void ChangeSong(int value) {
        _currentMusic = (_currentMusic + value) % _musics.Length ;
        _asMusic.clip = _musics[_currentMusic].AudioClip;
        _hudManager.SetMusicData(_musics[_currentMusic]);
        _asMusic.Play();
    }
}
