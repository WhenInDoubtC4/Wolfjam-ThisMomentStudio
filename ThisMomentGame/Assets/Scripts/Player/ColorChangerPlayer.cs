using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangerPlayer : MonoBehaviour
{
    [SerializeField]
    private Shader multicolorShader;

    [SerializeField]
    private SpriteRenderer targetSprtie;

    void Start()
    {
        if (!multicolorShader)
        {
            Debug.LogError("Multicolor shader is not set, things will not work!");
            return;
        }

        if (!targetSprtie)
        {
            Debug.LogError("Target sprite is not set, things will not work!");
            return;
        }

        StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        while (true)
        {
            AssignNewColor(new Color(0.164f, 0.86f, 0.3f));

            yield return new WaitForSeconds(0.5f);

            AssignNewColor(new Color(0.96f, 0.39f, 0.32f));

            yield return new WaitForSeconds(0.5f);

            AssignNewColor(new Color(0.96f, 0.77f, 0.42f));

            yield return new WaitForSeconds(0.5f);

            AssignNewColor(new Color(0.96f, 0.34f, 0.65f));

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AssignNewColor(Color color, float transitionDuration = 0.5f, float alpha = 1f)
    {
        Material oldMaterial = targetSprtie.material;
        Color oldColor = oldMaterial.GetColor("_NextColor");

        Material newColorMat = new(multicolorShader);

        newColorMat.name = "TransientColorMaterial";
        newColorMat.SetColor("_PreviousColor", oldColor);
        newColorMat.SetColor("_NextColor", color);
        newColorMat.SetFloat("_Alpha", alpha);
        newColorMat.SetFloat("_TransitionDuration", transitionDuration);
        newColorMat.SetFloat("_TransitionStart", Time.time);

        targetSprtie.material = newColorMat;
    }
}
