using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMessage : MonoBehaviour
{
    bool visible = true;
    private UnityEngine.UI.Text text;

    private void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            visible = !visible;
        }

        if(visible)
        {
            text.color = Color.white;
        }
        else
        {
            text.color = Color.clear;
        }
    }
}
