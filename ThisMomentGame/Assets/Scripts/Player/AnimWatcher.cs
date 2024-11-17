using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimWatcher : MonoBehaviour
{
    public UnityEvent animFinished = new();
    public void AnimationFinished()
    {
        Debug.Log("ANIMATION FINISHED");
        animFinished.Invoke();
    }
}
