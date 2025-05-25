using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTreeFPS
{
    public class CameraBob : MonoBehaviour
    {
        FPSController controller;
        Animator animator;
        InputManager manager;

        void Start()
        {
            controller = FindFirstObjectByType<FPSController>();
            animator = GetComponent<Animator>();
            manager = FindFirstObjectByType<InputManager>();
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetBool("isMoving", CheckMovement());

            if(!FPSController.crouch)
            animator.SetBool("Run", Input.GetKey(manager.Run));
        }

        public bool CheckMovement()
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                return true;
            }

            return false;
        }
    }
}
