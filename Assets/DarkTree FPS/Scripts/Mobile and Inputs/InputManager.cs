/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool _useMobileInput;

    [SerializeField]
    public static bool useMobileInput;
    
    [Header("Movement keys")]
    public KeyCode Crouch;
    public KeyCode Run;
    public KeyCode Jump;
    public KeyCode LeanLeft;
    public KeyCode LeanRight;

    [Header("Gameplay keys")]
    public KeyCode Fire;
    public KeyCode Aim;
    public KeyCode Use;
    public KeyCode DropWeapon;
    public KeyCode Reload;
    public KeyCode FiremodeSingle;
    public KeyCode FiremodeAuto;
    public KeyCode Inventory;

    public static Vector2 joystickInputVector;
    public static Vector2 touchPanelLook;

    //public UnityEngine.UI.Button[] mobileEquipmentButtons;

    private float playerBodyMovementSmoothness = 5f;

    private void Awake()
    {
        useMobileInput = _useMobileInput;

        /*
        if (useMobileInput)
        {
            foreach(UnityEngine.UI.Button button in mobileEquipmentButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (UnityEngine.UI.Button button in mobileEquipmentButtons)
            {
                button.gameObject.SetActive(false);
            }
        }*/
    }

    private void Update()
    {
        MovementDirection();
    }
    
    public static float horizontalFactor;
    public static float verticalFactor;
    
    public void MovementDirection()
    {
        horizontalFactor = Mathf.Lerp(horizontalFactor, Input.GetAxis("Horizontal"), Time.deltaTime * playerBodyMovementSmoothness);
        verticalFactor = Mathf.Lerp(verticalFactor, Input.GetAxis("Vertical"), Time.deltaTime * playerBodyMovementSmoothness);
    }

    public bool IsRunning()
    {
        return Input.GetKey(Run);
    }
}
