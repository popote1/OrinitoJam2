using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIMainMenu : MonoBehaviour
{
    public Button BpPlay;
    public Button Bpcontrol;
    public Button BpCredit;
    public Button BpControlReturn;
    public Button BpCreditReturn;
    public GameObject PanelControl;
    public GameObject PanelCredit;
    [Header("Sounds")]
    [Range(0,1)]public float OpenPanelVolume = 1;
    public AudioClip[] OpenPanelClips;
    [Range(0,1)]public float MenuMusicVolume = 1;
    public AudioClip MenuMusicClip;


    public void Start() {
        if (AudioManager.Instance != null && !AudioManager.Instance.IsPlayingMusic) {
            AudioManager.Instance.PlayMusic(MenuMusicClip, MenuMusicVolume);
        }
    }

    public void UIOpenControls() {
        PanelControl.SetActive(true);
        BpControlReturn.Select();
        if (AudioManager.Instance != null) {
            //AudioManager.Instance.PlaySFX(ClickClips[Random.Range(0,ClickClips.Length)], CLickVolume);
            AudioManager.Instance.PlaySFX(OpenPanelClips[Random.Range(0,OpenPanelClips.Length)], OpenPanelVolume);
        }
    }

    public void UICloseControls() {
        PanelControl.SetActive(false);
        Bpcontrol.Select();
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(OpenPanelClips[Random.Range(0,OpenPanelClips.Length)], OpenPanelVolume);
        }
    }

    public void UIOpenCredit() {
        PanelCredit.SetActive(true);
        BpCreditReturn.Select();
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(OpenPanelClips[Random.Range(0,OpenPanelClips.Length)], OpenPanelVolume);;
        }
    }

    public void UICloseCredit() {
        PanelCredit.SetActive(false);
        BpCredit.Select();
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(OpenPanelClips[Random.Range(0,OpenPanelClips.Length)], OpenPanelVolume);
        }
    }

    public void UIPlay() {
        SceneManager.LoadScene(1);
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(OpenPanelClips[Random.Range(0,OpenPanelClips.Length)], OpenPanelVolume);
        }
    }

}
