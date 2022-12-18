using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public PlayerScript PlayerScript;
    public Slider sliderGaz;
    public TMP_Text TxtGazText;

    [Header("GameOver")] public float GameOverFadeInTime = 0.5f;
    public CanvasGroup CanvasGroupPanelGameOver;
    [Header("AutoRadio")] 
    public GameObject PanelAutoRadio;
    public TMP_Text MusicName;

    public void SetGaz(float actualGaz, float maxGaz) {
        sliderGaz.value = actualGaz / maxGaz;
        TxtGazText.text = Mathf.RoundToInt(actualGaz) + "/" + Mathf.RoundToInt(maxGaz);
    }

    public void OpenGameOverPanel()
    {
        CanvasGroupPanelGameOver.transform.gameObject.SetActive(true);
        CanvasGroupPanelGameOver.DOFade(1, GameOverFadeInTime);
        PanelAutoRadio.SetActive(false);
    }

    public void SetMusicData(SOMusic music) {
        if (music == null) {
            MusicName.text = "-------------";
        }
        MusicName.text = music.Name;
    }

    public void UIPressPlay() {
        PlayerScript.RestartTheSong();
    }

    public void UIPressStrop() {
        PlayerScript.PauseTheSong();
    }

    public void UIPressNext() {
        PlayerScript.ChangeSong(+1);
    }
    public void UIPressPrev() {
        PlayerScript.ChangeSong(-1);
    }

    public void UIReturnToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
