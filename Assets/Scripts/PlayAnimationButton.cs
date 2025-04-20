using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayAnimationButton : MonoBehaviour
{
    public bool IsPlayed = false;

    public void PlayAnimation()
    {
        IsPlayed = true;
    }
}