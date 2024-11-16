using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [SerializeField] List<Image> uiElements;
    public float Duration { get { return duration; } set { duration = value; if (duration <= 0) duration = 0.0001f; } }
    [SerializeField] float duration = 1f;

    [SerializeField] Color fadeInColor = Color.white;
    [SerializeField] Color fadeOutColor = Color.white;

    [SerializeField] UnityEvent OnFadeInComplete;
    [SerializeField] UnityEvent OnFadeOutComplete;
    
    Coroutine fadeInCorutine = null;
    Coroutine fadeOutCorutine = null;

    public void FadeIn()
    {
        if(fadeInCorutine != null || fadeOutCorutine != null)
            return;

        fadeInCorutine = StartCoroutine(Fade(fadeInColor));
    }

    public void FadeOut()
    {
        if (fadeInCorutine != null || fadeOutCorutine != null)
            return;

        fadeOutCorutine = StartCoroutine(Fade(fadeOutColor));
    }
    
    IEnumerator Fade(Color targetColor)
    {
        float tick = 0.02f;
        WaitForSeconds delay = new WaitForSeconds(tick);
        float timer = 0f;

        List<Color> elementColors = new List<Color>();

        foreach(Image image in uiElements)
        {
            Color color = image.color;
            elementColors.Add(color); 
        }


        while (elementColors[elementColors.Count - 1] != targetColor)
        {
            for(int i = 0; i < elementColors.Count; i++)
            {
                elementColors[i] = Color.Lerp(elementColors[i], targetColor, Mathf.Clamp01(timer / duration));
                uiElements[i].color = elementColors[i];
            }
            
            timer += tick;
            yield return delay;
        }

        FadeComplete();
    }

    void FadeComplete()
    {
        if (fadeOutCorutine != null)
            OnFadeOutComplete?.Invoke();

        if (fadeInCorutine != null)
            OnFadeInComplete?.Invoke();

        fadeOutCorutine = null;
        fadeInCorutine = null;
    }
}
