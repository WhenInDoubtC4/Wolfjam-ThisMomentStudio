using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    [SerializeField] string triggerTag;
    [SerializeField] UnityEvent triggerEvents;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != triggerTag)
            return;

        triggerEvents?.Invoke();
    }
}
