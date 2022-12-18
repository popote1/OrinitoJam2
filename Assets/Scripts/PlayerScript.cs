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
    [SerializeField] private float _maxGaz=70;
    [SerializeField] private float _gazDecrease = 1;

    [SerializeField] private SOMusic[] _musics;
    [SerializeField] private AudioSource _asMusic;
    [Range(0,1)]public float WalkManSFXVolume = 1;
    public AudioClip[] WalkManSFXClips;
    [Range(0,1)]public float GameOverMusicVolume = 1;
    public AudioClip GameoverMusicClips;
    
    
    

    public static bool[] Choises = new bool[255];
    
    public float Gaz
    {
        get => _gaz;
        set {
            _gaz=value;
            if (_gaz > _maxGaz) _gaz = _maxGaz;
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
    private bool _musicwasOn;
    private float _musicTimer;

    public static bool IsConditionValue(int index) {
        if (index == 0) return true;
        return Choises[index];
    }
    void Start() {
        Gaz = _maxGaz;
        if (AudioManager.Instance !=null&&_musics != null && _musics.Length > 0) {
            AudioManager.Instance.PlayMusic(_musics[0].AudioClip);
            _currentMusic = 0;
            _hudManager.SetMusicData(_musics[0]);
            _musicIsPlaying = true;
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
                
                PauseTheSong();
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
        if(AudioManager.Instance!=null)   AudioManager.Instance.PlayMusic(GameoverMusicClips , GameOverMusicVolume);
    }

    public void QuitDiscution(bool triggerIsRemove = false) {
        _carController.CanControl = true;
        _isInDiscution = false;
        if(_musicwasOn)RestartTheSong();
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

    public void ChangeSong(int value) {
        if (AudioManager.Instance == null) return;
        _currentMusic = (_currentMusic + value) % _musics.Length ;
        if (_currentMusic < 0) _currentMusic = _musics.Length - 1;
        AudioManager.Instance.PlayMusic(_musics[_currentMusic].AudioClip);
        _hudManager.SetMusicData(_musics[_currentMusic]);
        AudioManager.Instance.PlaySFX(WalkManSFXClips[Random.Range(0,WalkManSFXClips.Length)],WalkManSFXVolume);
        
    }

    public void PauseTheSong()
    {
        if (_musicIsPlaying) _musicwasOn=true;
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PauseMusic();
        _musicIsPlaying = true;
        AudioManager.Instance.PlaySFX(WalkManSFXClips[Random.Range(0,WalkManSFXClips.Length)],WalkManSFXVolume);
    }

    public void RestartTheSong()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.RestartMusic();
        _musicIsPlaying = true;
        AudioManager.Instance.PlaySFX(WalkManSFXClips[Random.Range(0,WalkManSFXClips.Length)],WalkManSFXVolume);
    }
}
