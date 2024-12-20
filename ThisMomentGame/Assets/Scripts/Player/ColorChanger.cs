using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
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
