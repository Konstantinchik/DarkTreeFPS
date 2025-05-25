/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTreeFPS {

    public class Body : MonoBehaviour
    {
        public Animator bodyAnimator;
        public InputManager inputManager;
        public FPSController controller;

        private float bodyStandingYPos;
        public float bodyCrouchYShift = 0.5f;
        public float bodyCrouchSizeMultiplyer = 0.7f;
        private Vector3 bodyStandardScale;
        private Vector3 calculatedCrouchScale;

        Transform bodyTransform;

        Transform cameraHolder;
        
        void Start()
        {
            if(InputManager.useMobileInput)
            {
                gameObject.SetActive(false);
                return;
            }

            cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();
            inputManager = FindFirstObjectByType<InputManager>();
            controller = FindFirstObjectByType<FPSController>();
            bodyTransform = bodyAnimator.GetComponent<Transform>();

            bodyStandingYPos = bodyTransform.localPosition.y;
            bodyStandardScale = bodyTransform.localScale;

            calculatedCrouchScale = bodyStandardScale * bodyCrouchSizeMultiplyer;
        }



        public bool IsMoving()
        {
            if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                return true;
            }

            return false;
        }

        Vector3 prevRot;
        Vector3 newRot;
        public float turnSpeed = 3;
        float turn;

        // Update is called once per frame
        void Update()
        {
            if(cameraHolder == null)
            {
                cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();
            }

            bodyAnimator.SetBool("IsMoving", IsMoving());
            
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            
            if (cameraHolder.transform.rotation.eulerAngles.y > prevRot.y + 10 || cameraHolder.transform.rotation.eulerAngles.y < prevRot.y - 10)
            {
                prevRot = cameraHolder.transform.rotation.eulerAngles;
                newRot = new Vector3(0, prevRot.y, 0);
                turn = mouseDelta.normalized.magnitude;
            }
            

            if(turn > 0)
            turn -= Time.deltaTime * 2;
            if(turn < 0)
            turn += Time.deltaTime * 2;

            bodyAnimator.SetFloat("Turn", Mathf.Lerp(bodyAnimator.GetFloat("Turn"), turn, Time.deltaTime * turnSpeed * 2));

            if (!IsMoving())
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRot), Time.deltaTime * turnSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, cameraHolder.transform.rotation.eulerAngles.y, 0);
            }
            
            if(!controller.isGrounded())
            {
                bodyAnimator.SetBool("OnGround", false);
            }
            else
            {
                bodyAnimator.SetBool("OnGround", true);
            }

            if (FPSController.crouch)
            {
                bodyAnimator.SetBool("Crouch", true);
                bodyAnimator.SetFloat("Forward", InputManager.verticalFactor);
                bodyAnimator.SetFloat("Horizontal", InputManager.horizontalFactor);
                bodyTransform.localScale = calculatedCrouchScale;
                bodyTransform.localPosition = new Vector3(bodyTransform.localPosition.x, bodyCrouchYShift, bodyTransform.localPosition.z);
            }else
            {
                bodyAnimator.SetBool("Crouch", false);
                bodyTransform.localScale = bodyStandardScale;
                bodyTransform.localPosition = new Vector3(bodyTransform.localPosition.x, bodyStandingYPos, bodyTransform.localPosition.z);
            }

            if (!inputManager.IsRunning() && !FPSController.crouch)
            {
                bodyAnimator.SetFloat("Forward", InputManager.verticalFactor/2);
                bodyAnimator.SetFloat("Horizontal", InputManager.horizontalFactor/2);
            }
            else if(inputManager.IsRunning() && !FPSController.crouch)
            {
                bodyAnimator.SetFloat("Forward", InputManager.verticalFactor);
                bodyAnimator.SetFloat("Horizontal", InputManager.horizontalFactor);
            }
    }
    }
}