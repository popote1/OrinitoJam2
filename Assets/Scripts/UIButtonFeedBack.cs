using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class UIButtonFeedBack : MonoBehaviour ,IPointerEnterHandler , IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public float AnimationTime = 0.3f;
    [Header("Scale Options")]
    public bool UsScale=true;
    public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public float EndSize =1.1f;

    [Header("Rotation Options")]
    public bool UsRotation=true;
    public Vector3 EndRotation = new Vector3(0,0,2);

    [Header("Sounds")] 
    [Range(0,1)]public float AnimInvolume = 1;
    public AudioClip[] AnimInClps;
    [Range(0,1)]public float AnimOutvolume = 1;
    public AudioClip[] AnimOutClps;
    // Start is called before the first frame update

    private void PlayAnimationIn()
    {
        transform.DOPause();
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        if (UsScale) transform.DOScale(EndSize, AnimationTime).SetEase(AnimationCurve);
        if(UsRotation)transform.DORotate(EndRotation, AnimationTime);
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(AnimInClps[Random.Range(0,AnimInClps.Length)], AnimInvolume);
        }
    }
    private void PlayAnimationOut()
    {
        transform.DOPause();
        transform.eulerAngles = EndRotation;
        transform.localScale = Vector3.one*EndSize;
        if (UsScale) transform.DOScale(1, AnimationTime).SetEase(AnimationCurve);
        if(UsRotation)transform.DORotate(Vector3.zero, AnimationTime);
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(AnimOutClps[Random.Range(0,AnimOutClps.Length)], AnimOutvolume);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayAnimationIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       PlayAnimationOut();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayAnimationIn();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        PlayAnimationOut();
    }
    
    
}
