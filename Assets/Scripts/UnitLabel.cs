using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class UnitLabel : MonoBehaviour
{
    public TMP_Text unitIdLabel;
    public TMP_Text unitIdArrowLabel;
    public TMP_Text hpLabel;


    void Start()
    {
        unitIdArrowLabel.transform.DOScaleY(1.2f, 0.9f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }

    public void SetFocus(bool focus)
    {
        unitIdArrowLabel.enabled = focus;
    }
}