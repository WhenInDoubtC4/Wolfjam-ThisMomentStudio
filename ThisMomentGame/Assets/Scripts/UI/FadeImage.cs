using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeImage: MonoBehaviour
{
    [SerializeField] Image sprite;

    public Color TargetColor { get { return targetColor; } set { targetColor = value; } }
    [SerializeField] Color targetColor = Color.white;

    public float Duration { get { return duration; } set { duration = value; if (duration <= 0) duration = 0.0001f; } }
    [SerializeField] float duration = 1f;

    [SerializeField] UnityEvent OnFadeComplete;
    Coroutine fadeCorutine = null;

    private void OnValidate()
    {
        if(sprite != null)
            return;

        if(transform.TryGetComponent(out Image selfRenderer))
            sprite = selfRenderer;
    }

    public void TriggerFade()
    {
        if(fadeCorutine != null)
            return;

        fadeCorutine = StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float tick = 0.02f;
        WaitForSeconds delay = new WaitForSeconds(tick);
        float timer = 0f;

        Color color = sprite.color;

        while (color != targetColor)
        {
            color = Color.Lerp(color, targetColor, Mathf.Clamp01(timer/duration));
            sprite.color = color;

            timer += tick;
            yield return delay;
        }

        FadeComplete();
    }

    void FadeComplete()
    {
        fadeCorutine = null;
        OnFadeComplete?.Invoke();
    }
}
