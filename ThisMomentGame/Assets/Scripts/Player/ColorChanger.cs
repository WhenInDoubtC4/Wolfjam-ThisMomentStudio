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

        //TODO: Remove this later
        StartCoroutine(TEMP_TestColorChange());
    }

    IEnumerator TEMP_TestColorChange()
    {
        AssignNewColor(Color.red);

        yield return new WaitForSeconds(2f);

        AssignNewColor(Color.blue);

        yield return new WaitForSeconds(2f);

        AssignNewColor(new Color(0.5f, 0.5f, 0f));

        yield return new WaitForSeconds(2f);

        AssignNewColor(Color.green);
    }

    private void AssignNewColor(Color color, float alpha = 1f)
    {
        Material newColorMat = new(multicolorShader);

        newColorMat.name = "TransientColorMaterial";
        newColorMat.SetColor("_Color", color);
        newColorMat.SetFloat("_Alpha", alpha);

        targetSprtie.material = newColorMat;
    }
}
