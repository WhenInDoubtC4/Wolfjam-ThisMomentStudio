using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimedTrigger : MonoBehaviour
{
    [SerializeField] int uses = 0;
    [SerializeField] UnityEvent events;
    [SerializeField] PlayableDirector intro; 
    
    Coroutine run = null;
    int used = 0;

    public void StartCountdown(float delay)
    {
        if (run == null)
        {
            if(uses > used || uses == 0)
            {
                used++;
                run = StartCoroutine(Countdown(delay));
            }
        }
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
        intro.Play();
    }
}
