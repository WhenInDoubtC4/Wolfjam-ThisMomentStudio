using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowKeys : MonoBehaviour
{
    TMP_Text text;
    private void Start()
    {
        //gameObject.SetActive(value);
        text = GetComponent<TMP_Text>();
        text.enabled = false;
    }
    public void showText(bool value)
    {
        text.enabled = value;
        //gameObject.SetActive(value);
    }
}
