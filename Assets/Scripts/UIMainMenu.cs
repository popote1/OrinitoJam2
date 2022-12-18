using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public Button BpPlay;
    public Button Bpcontrol;
    public Button BpCredit;
    public Button BpControlReturn;
    public Button BpCreditReturn;
    public GameObject PanelControl;
    public GameObject PanelCredit;


    public void UIOpenControls() {
        PanelControl.SetActive(true);
        BpControlReturn.Select();
    }

    public void UICloseControls() {
        PanelControl.SetActive(false);
        Bpcontrol.Select();
    }

    public void UIOpenCredit() {
        PanelCredit.SetActive(true);
        BpCreditReturn.Select();
    }

    public void UICloseCredit() {
        PanelCredit.SetActive(false);
        BpCredit.Select();
    }

    public void UIPlay() {
        SceneManager.LoadScene(1);
    }

}
