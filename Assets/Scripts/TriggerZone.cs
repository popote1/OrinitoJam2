using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public GameObject InteractableIdicator;
    public SODialogue Dialogue;

    public bool IsInteractive;
    // Start is called before the first frame update
    void Start() {
        if (Dialogue == null) {
            Debug.LogWarning("Dialogue non Attribuer dans l'objet "+transform.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerScript>().SetTriggerZone(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerScript>().ExitTriggerZone(this);
        }
    }

    public void ShowInteractable(bool value) {
        if (value)
        {
            if (!IsInteractive)
            {
               transform.DOScale(transform.localScale*1.1f, 1).SetLoops(-1, LoopType.Yoyo);
               IsInteractive = true;
            }
        }
        else
        {
            IsInteractive = false;
           transform.DOPause();
            
        }
    }
    
}
