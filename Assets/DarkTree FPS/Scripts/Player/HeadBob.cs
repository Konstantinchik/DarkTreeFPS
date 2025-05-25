using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTreeFPS {

    public class HeadBob : MonoBehaviour
    {
        Animator animator;
        InputManager input;
        FPSController controller;

        public float runHeadBobSpeed = 2;


        private void Start()
        {
            animator = GetComponent<Animator>();
            input = FindFirstObjectByType<InputManager>();
            controller = FindFirstObjectByType<FPSController>();
        }

        private void Update()
        {
            if (animator == null)
                return;

            if (controller.isGrounded() && FPSController.canMove == true)
            {
                animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
                animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

                animator.SetBool("Run", input.IsRunning());
            }
            else
            {
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 0);

                animator.SetBool("Run", false);
            }
        }
    }
}