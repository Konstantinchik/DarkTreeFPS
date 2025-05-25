using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VisionUI : MonoBehaviour
{
    public static float visionAmount;
    public Image image;
    public Image eye;

    private void Update()
    {
        if(visionAmount > 0)
            visionAmount -= Time.deltaTime;

        eye.color = new Color(1, 1, 1, visionAmount * 0.33f);
        image.fillAmount = visionAmount * 0.33f;
    }
}
