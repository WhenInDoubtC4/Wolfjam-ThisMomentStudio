using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenPunch : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float scale = 0.2f;
    [SerializeField] float duration = 0.3f;
    [SerializeField] int vibrations = 2;
    [SerializeField, Range(0, 1)] float elastic = 0.5f;

    float timer;

    private void OnValidate()
    {
        if(target == null && TryGetComponent(out Transform tForm))
            target = tForm;
    }

    private void FixedUpdate()
    {
        if (timer > 0)
            timer -= Time.fixedDeltaTime;
    }

    public void TriggerPunch()
    {
        if (target == null || timer > 0)
            return;

        target.DOPunchScale(target.localScale * scale, duration, vibrations, elastic);
        timer = duration;
    }
}
