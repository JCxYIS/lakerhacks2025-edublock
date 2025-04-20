using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    public GameObject btn1;
    public GameObject btn2;

    void Start()
    {
        btn1.transform.DOScale(1.1f, .4f)
            .SetEase(Ease.Flash)
            .SetLoops(-1, LoopType.Yoyo);

        btn2.transform.DOScale(1.1f, .4f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}