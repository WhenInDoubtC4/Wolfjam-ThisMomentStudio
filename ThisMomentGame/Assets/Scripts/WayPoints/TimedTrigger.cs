using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent events;
    Coroutine run = null;

    public void StartCountdown(float delay)
    {
        if(run == null)
            run = StartCoroutine(Countdown(delay));
    }

    IEnumerator Countdown(float delay)
    {
        float tick = 0.02f;
        WaitForSeconds ret = new WaitForSeconds(tick);
        float timer = 0;

        while(timer < delay)
        {
            yield return ret;
            timer += tick;
        }

        events?.Invoke();
    }
}
