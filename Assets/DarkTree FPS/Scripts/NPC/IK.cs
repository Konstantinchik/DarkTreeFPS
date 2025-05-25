/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;

namespace DarkTreeFPS
{

    public class IK : MonoBehaviour
    {
        protected Animator animator;

        public bool ikActive = false;
        public Transform leftHandObj = null;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void OnAnimatorIK()
        {
            if (animator)
            {
                if (ikActive)
                {
                    if (leftHandObj != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                    }

                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }
    }
}