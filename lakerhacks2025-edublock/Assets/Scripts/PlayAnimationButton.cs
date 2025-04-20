using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayAnimationButton : MonoBehaviour
{
    public bool IsPlayed = false;

    void Start()
    {
        transform.DOScale(1.15f, 0.9f).SetLoops(-1, LoopType.Yoyo);
    }

    public void PlayAnimation()
    {
        IsPlayed = true;
    }
}