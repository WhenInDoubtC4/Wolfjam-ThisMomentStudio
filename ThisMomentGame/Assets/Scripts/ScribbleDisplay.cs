using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ScribbleDisplay : MonoBehaviour
{
    private SpriteRenderer scribbleSprite;

    [SerializeField] private float fadeInDuration;
    private float fadeInTimer;
    private bool fadingIn;

    [SerializeField] private float displayDurartion;
    private float displayTimer;
    private bool displaying;

    [SerializeField] private float fadeOutDuration;
    private float fadeOutTimer;
    private bool fadingOut;

    private void Start()
    {
        scribbleSprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        FadeIn();
        Displaying();
        FadingOut();
    }

    public void StartEffect()
    {
        fadingIn = true;
        fadeInTimer = 0;
    }
    private void FadeIn()
    {
        if(fadingIn)
        {
            fadeInTimer += Time.deltaTime;
            float completetion = fadeInTimer / fadeInDuration;
            scribbleSprite.color = new Color(scribbleSprite.color.r, scribbleSprite.color.g, scribbleSprite.color.b, completetion);
            if(fadeInTimer >= fadeInDuration)
            {
                fadingIn = false;
                displaying = true;
                displayTimer = displayDurartion;
            }
        }
    }
    private void Displaying()
    {
        if (displaying)
        {
            Debug.Log(displayTimer);
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0)
            {
                displaying = false;
                fadingOut = true;
                fadeOutTimer = fadeOutDuration;
            }
        }
    }
    private void FadingOut()
    {
        if (fadingOut)
        {
            fadeOutTimer -= Time.deltaTime;
            float completetion = fadeOutTimer / fadeOutDuration;
            scribbleSprite.color = new Color(scribbleSprite.color.r, scribbleSprite.color.g, scribbleSprite.color.b, completetion);
            if (fadeOutTimer <= 0)
            {
                fadingOut = false;
            }
        }
    }
}
