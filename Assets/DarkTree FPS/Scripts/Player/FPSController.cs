/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using System.Collections.Generic;
using DTInventory;

namespace DarkTreeFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 1f;
        public float crouchSpeed = 0.4f;
        public float runSpeedMultiplier = 2f;
        public float jumpForce = 4f;
        
        public float crouchHeight = 0.5f;

        [HideInInspector]
        public static bool crouch = false;
        public bool lockCursor;
        
        PlayerStats playerStats;
        private CharacterController characterController;
        public Transform camHolder;
        private float moveSpeedLocal;
        
        private float distanceToGround;

        private Animator weaponHolderAnimator;

        [HideInInspector]
        public Animator mainCameraAnimator;

        private WeaponManager weaponManager;

        [HideInInspector]
        public bool isClimbing = false;

        private float inAirTime;

        [HideInInspector]
        public bool mouseLookEnabled = true;

        //Velocity calculation variable
        private Vector3 previousPos = new Vector3();
        
        Vector3 dirVector;

        InputManager inputManager;

        private float defaultColliderHeight;

        public static bool canMove = true;

        [Header("MouseLook Settings")]

        private float clampY = 160;
        public Vector2 sensitivity = new Vector2(0.5f, 0.5f);
        public Vector2 smoothing = new Vector2(3, 3);

        [HideInInspector]
        public Vector2 targetDirection;
        [HideInInspector]
        public Vector2 _mouseAbsolute;
        [HideInInspector]
        public Vector2 _smoothMouse;

        [Header("Landing and air")]
        public AudioClip softLandingSound;
        public AudioClip hardLandingSound;
        
        public float hardLandingTime = 2f;

        private AudioSource audioSource;
        private float airTime;

        public static bool isSwimming;

        [HideInInspector]
        public bool hitTheGround = true;
        
        [HideInInspector]
        public static float highestTriggerEdge = -9999;

        private void OnEnable()
        {
            if (!InputManager.useMobileInput)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }

            isSwimming = false;

            characterController = GetComponent<CharacterController>();

            defaultColliderHeight = GetComponent<CharacterController>().height;

            distanceToGround = GetComponent<CharacterController>().bounds.extents.y;
            weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();
            mainCameraAnimator = Camera.main.GetComponent<Animator>();

            playerStats = FindFirstObjectByType<PlayerStats>();
            audioSource = GetComponent<AudioSource>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
            inputManager = FindFirstObjectByType<InputManager>();
        }

        [HideInInspector]
        public Vector2 recoil;

        Vector3 moveDirection;
        public float gravity = 10f;

        private bool lastSwimmingState;

        private void Update()
        {
            if (characterController == null)
                GetComponent<CharacterController>();

            if (!isSwimming)
            {
                if (!isGrounded())
                {
                    airTime += Time.deltaTime;
                }
                else
                {
                    if (airTime > hardLandingTime)
                    {
                        mainCameraAnimator.Play("HardLanding");
                        playerStats.ApplyDamage((int)(playerStats.minFailingDamage * airTime));
                        if (hardLandingSound != null)
                            audioSource.PlayOneShot(hardLandingSound);

                    }
                    else if (airTime > 0.3f)
                    {
                        mainCameraAnimator.Play("SoftLanding");
                        if (softLandingSound != null)
                            audioSource.PlayOneShot(softLandingSound);
                    }

                    airTime = 0;
                }
            }
            
            if (!canMove)
                return;
            
            if (!InputManager.useMobileInput)
            {
                StandaloneMovement();
            }
            else
            {
                MobileMovement();
            }
       
            Crouch();
            Landing();

            CharacterMovement();
            if (mouseLookEnabled && !InventoryManager.showInventory && lockCursor)
                MouseLook();

            if (!isSwimming && !isClimbing)
            {
                if (isGrounded() && playerStats.stamina > 5 && Input.GetButtonDown("Jump"))
                {
                    playerStats.stamina -= 5;
                    moveDirection.y = jumpForce;
                }
                    characterController.Move(moveDirection * Time.deltaTime);
            }

            if (isSwimming && !isGrounded() && hitTheGround)
            {
                hitTheGround = false;
            }


            if (isSwimming && lastSwimmingState == false)
            {
                lastSwimmingState = true;
            }

            if(!isSwimming && lastSwimmingState == true)
            {
                weaponHolderAnimator.Play("Unhide");
                lastSwimmingState = false;
            }

            if(weaponManager.activeWeapon != null)
                weaponHolderAnimator.SetBool("Reloading",weaponManager.activeWeapon.reloading);
        }

        void MouseLook()
        {
            Quaternion targetOrientation = Quaternion.Euler(targetDirection);

            Vector2 mouseDelta = new Vector2();

            if (InputManager.useMobileInput)
            {
                mouseDelta = new Vector2(InputManager.touchPanelLook.x * Time.deltaTime, InputManager.touchPanelLook.y * Time.deltaTime);
            }
            else if (!InputManager.useMobileInput)
            {
                mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
            
            _mouseAbsolute += _smoothMouse;

            if (recoil != Vector2.zero)
            {
                _mouseAbsolute.x += recoil.x;
                _mouseAbsolute.y += recoil.y;
                recoil = Vector2.zero;
            }

            if (_mouseAbsolute.y < -clampY * 0.5f)
                _mouseAbsolute.y = -clampY * 0.5f;

            if (_mouseAbsolute.y > clampY * 0.5f)
                _mouseAbsolute.y = clampY * 0.5f;
            
            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            camHolder.transform.localRotation = xRotation;

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, camHolder.transform.InverseTransformDirection(Vector3.up));
            camHolder.transform.localRotation *= yRotation;
            camHolder.transform.rotation *= targetOrientation;
        }

        public float distanceToWallRaiseWeaponUp = 0.1f;

        public bool PlayerCloseToWall()
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * distanceToWallRaiseWeaponUp, out hit, distanceToWallRaiseWeaponUp))
            {
                return true;
             
            }
            else
            {
                return false;
            }
        }

        void StandaloneMovement()
        {
            if (canMove == false)
                return;

            if(weaponHolderAnimator == null)
            {
                weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();
            }

            if (isGrounded())
            {
                weaponHolderAnimator.SetBool("RaiseWeapon", PlayerCloseToWall());

                weaponHolderAnimator.SetBool("InAir", false);

                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Walk", true);
                    moveSpeedLocal = moveSpeed;
                }
                else
                    weaponHolderAnimator.SetBool("Walk", false);

                if (Input.GetKey(inputManager.Run)&& playerStats.stamina > 0 && !isClimbing && !crouch && weaponHolderAnimator.GetBool("Walk") == true)
                {
                    moveSpeedLocal = runSpeedMultiplier * moveSpeed;
                    weaponHolderAnimator.SetBool("Run", true);
                }
                else
                    weaponHolderAnimator.SetBool("Run", false);
            }
            else
            {
                weaponHolderAnimator.SetBool("InAir", true);
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
            }

            if (crouch)
            {
                moveSpeedLocal = crouchSpeed;
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Crouch", true);
                }
                else
                    weaponHolderAnimator.SetBool("Crouch", false);
            }
            else
                weaponHolderAnimator.SetBool("Crouch", false);

            if (Input.GetKeyDown(inputManager.Crouch) && !isSwimming)
            {
                crouch = !crouch;
            }
            
            if (Input.GetKeyDown(inputManager.Jump) && !isSwimming)
            {
                crouch = false;
            }
            
        }
        
        public void MobileMovement()
        {
            if (isGrounded())
            {
                weaponHolderAnimator.SetBool("InAir", false);

                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Walk", true);
                    moveSpeedLocal = moveSpeed;
                }
                else
                    weaponHolderAnimator.SetBool("Walk", false);

                if (InputManager.joystickInputVector.y > 0.5f && !isClimbing && !crouch && weaponHolderAnimator.GetBool("Walk") == true)
                {
                    moveSpeedLocal = runSpeedMultiplier * moveSpeed;
                    weaponHolderAnimator.SetBool("Run", true);
                }
                else
                    weaponHolderAnimator.SetBool("Run", false);
            }
            else
            {
                weaponHolderAnimator.SetBool("InAir", true);
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
            }

            if (crouch)
            {
                moveSpeedLocal = crouchSpeed;
                weaponHolderAnimator.SetBool("Walk", false);
                weaponHolderAnimator.SetBool("Run", false);
                if (CheckMovement())
                {
                    weaponHolderAnimator.SetBool("Crouch", true);
                }
                else
                    weaponHolderAnimator.SetBool("Crouch", false);
            }
            else
                weaponHolderAnimator.SetBool("Crouch", false);

            if (Input.GetKeyDown(inputManager.Crouch))
            {
                crouch = !crouch;
            }
            if (Input.GetKeyDown(inputManager.Jump))
            {
                crouch = false;
            }
            
        }
        
        void CharacterMovement()
        {
            if (canMove == false)
                return;

            if(weaponHolderAnimator == null)
            {
                weaponHolderAnimator = GameObject.Find("Weapon holder").GetComponent<Animator>();
            }

            var camForward = camHolder.transform.forward;
            var camRight = camHolder.transform.right;

            if (!isSwimming && hitTheGround)
            {
                camForward.y = 0f;
                camRight.y = 0f;
            }
            
            camForward.Normalize();
            camRight.Normalize();

            if (isSwimming)
            {
                gravity = 0.05f;
            }
            else
            {
                gravity = 10;
            }

            //Gravity!!!
            if(!isClimbing)
            moveDirection.y -= gravity * Time.deltaTime;

            if (isClimbing && !isSwimming)
            {
                crouch = false;

                weaponHolderAnimator.SetBool("HideWeapon", true);
                
                if (!InputManager.useMobileInput)
                {
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }
                else if(InputManager.useMobileInput)
                {
                    dirVector = new Vector3(InputManager.joystickInputVector.x, 0, InputManager.joystickInputVector.y);
                }
                
                Vector3 verticalDirection = transform.up;
                Vector3 moveDirection = (verticalDirection) * dirVector.z+ camRight * dirVector.x;
                
                characterController.Move(transform.position + moveDirection * moveSpeedLocal * Time.deltaTime);
            }
            else if(isSwimming && !isClimbing)
            {
                weaponHolderAnimator.SetBool("HideWeapon", true);

                if (!InputManager.useMobileInput)
                {
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }
                else if(InputManager.useMobileInput)
                {
                    dirVector = new Vector3(InputManager.joystickInputVector.x, 0, InputManager.joystickInputVector.y);
                }
                Vector3 moveDirection = camForward * dirVector.z + camRight * dirVector.x;

                characterController.Move(moveDirection * moveSpeedLocal * Time.deltaTime);
            }
            else if(!isSwimming && !isClimbing)
            {
                weaponHolderAnimator.SetBool("HideWeapon", false);

                if (!InputManager.useMobileInput)
                {
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
                }
                else if (InputManager.useMobileInput)
                {
                    dirVector = new Vector3(InputManager.joystickInputVector.x, 0, InputManager.joystickInputVector.y);
                }
                Vector3 moveDirection = camForward * dirVector.z + camRight * dirVector.x;

                characterController.Move(moveDirection * moveSpeedLocal * Time.deltaTime);
            }

            //print("Is Swimming : " + isSwimming + "/n" + "highestTriggerEdge : " + highestTriggerEdge + "/n" + "hitTheGround : " + hitTheGround);
            
        }

        public static bool CheckMovement()
        {
            if (!InputManager.useMobileInput)
            {
                if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0 || Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
                {
                    return true;
                }
            }

            if(InputManager.useMobileInput)
            {
                if (InputManager.joystickInputVector.x > 0 || InputManager.joystickInputVector.x < 0 || InputManager.joystickInputVector.y > 0 || InputManager.joystickInputVector.y < 0)
                    return true;
            }

            return false;
        }
        
        void Crouch()
        {
            if (crouch == true)
            {
                characterController.height = Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * 8);
                camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, new Vector3(0, 0.2f, 0), Time.deltaTime * 4 );
                //Ray ray = new Ray();
                //ray.origin = transform.position;
                //ray.direction = transform.up;
            }
            else
            {
                characterController.height = Mathf.Lerp(characterController.height, defaultColliderHeight, Time.deltaTime * 8);
                camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, new Vector3(0, 0.4f, 0), Time.deltaTime * 2);
            }
        }
        
        public float GetVelocityMagnitude()
        {
            var velocity = ((transform.position - previousPos).magnitude) / Time.deltaTime;
            previousPos = transform.position;
            return velocity;
        }
        
        public void CrouchMobile()
        {
            crouch = !crouch;
        }

        private bool haveCollision = false;

        public bool isGrounded()
        {
            haveCollision = characterController.isGrounded;

            if (!haveCollision)
            {
                return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.5f);
            }
            else
            {
                hitTheGround = true;
            }

            return true;
        }

        void Landing()
        {
            if(!isGrounded())
            {
                inAirTime += Time.deltaTime;
            }
            else
            {
                if (inAirTime > 0.5f)
                    weaponHolderAnimator.Play("Landing");

                inAirTime = 0;
            }
        }
    }
}