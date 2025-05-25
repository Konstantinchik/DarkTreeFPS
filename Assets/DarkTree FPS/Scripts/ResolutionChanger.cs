using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionChanger : MonoBehaviour
{
    public bool fullscreen = true;
    public Slider qualitySlider;

    void Start()
    {
        var dropdown = GetComponent<Dropdown>();

        List<string> options = new List<string>();

        int currentResolutionIndex = -1;

        Resolution[] resolutions = Screen.resolutions;

        for(int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString());
            
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        dropdown.AddOptions(options);

        dropdown.value = currentResolutionIndex;
    }

    public void ChangeResolution()
    {
        var dropdown = GetComponent<Dropdown>();
        var resolutionString = dropdown.options[dropdown.value].text;
        print(dropdown.options[dropdown.value].text);

        print(resolutionString.Split('x')[0] + " : " + resolutionString.Split('x')[1]);
        Screen.SetResolution(int.Parse(resolutionString.Split('x')[0]), int.Parse(resolutionString.Split('x')[1]), fullscreen);
    }
}
