﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.AI;

namespace DarkTreeFPS
{
    public class ZombieNPC : MonoBehaviour
    {
        public float health = 100f;
        public int maxDamage = 10;

        public bool isAgressive;
        
        public AudioClip roar;
        public AudioClip[] attackSounds;

        [Header("Wandering parameters")]
        public float wanderingSpeed = 0.6f;
        public float wanderingTimer = 13;
        public float wanderingRadius = 10;

        public float attackDistance = 3f;
        public GameObject zombieRagdoll;
        public float runSpeed = 0.9f;

        float timerW;

        bool isVisible;
        NavMeshAgent agent;
        Animator animator;
        AudioSource audioSource;
        NPCVision vision;

        public bool notRoared = true;

        CapsuleCollider capsule;
        List<ZombieNPC> friendsList; // warning CS0649: Field 'ZombieNPC.friendsList' is never assigned to, and will always have its default value null

        private GameObject target; // warning CS0169: The field 'ZombieNPC.target' is never used
        private Transform player;

        float timer; // warning CS0169: The field 'ZombieNPC.timer' is never used

        [HideInInspector]
        public bool isWorried = false;

        private Animator playerCameraAnimator;

        #region Character controller variables

        [Header("Character controller variables")]
        [Header("Same as in the ThirdPersonController")]

        public float m_MovingTurnSpeed = 360;
        public float m_StationaryTurnSpeed = 180;
        public float m_JumpPower = 12f;
        [Range(1f, 4f)]
        public float m_GravityMultiplier = 2f;
        public float m_RunCycleLegOffset = 0.2f;
        public float m_MoveSpeedMultiplier = 1f;
        public float m_AnimSpeedMultiplier = 1f;
        public float m_GroundCheckDistance = 0.1f;

        private Rigidbody m_Rigidbody;
        private bool m_IsGrounded;
        private float m_OrigGroundCheckDistance;
        private const float k_Half = 0.5f;
        private float m_TurnAmount;
        private float m_ForwardAmount;
        private Vector3 m_GroundNormal;
        private float m_CapsuleHeight;
        private Vector3 m_CapsuleCenter;
        private CapsuleCollider m_Capsule;

        private bool m_Crouching;
        [HideInInspector]
        private bool lookAtTarget = false;
        #endregion

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            agent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = Object.FindFirstObjectByType<PlayerStats>().transform;
            capsule = GetComponent<CapsuleCollider>();
            vision = GetComponentInChildren<NPCVision>();
            playerCameraAnimator = Camera.main.GetComponent<Animator>();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            agent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = Object.FindFirstObjectByType<PlayerStats>().transform;
            capsule = GetComponent<CapsuleCollider>();
            playerCameraAnimator = Camera.main.GetComponent<Animator>();

            StartCoroutine(CheckFriends());

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = true;
            }

            GetComponent<Collider>().enabled = true;

            //Controller variables
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        private void Update()
        {
            if (health > 0)
            {
                if (Vector3.Distance(transform.position, player.position) < 25 || isWorried)
                {
                    if (isAgressive)
                    {
                        Shout();
                        Roar();
                        agent.speed = runSpeed;
                        agent.SetDestination(player.position);
                        if (Vector3.Distance(transform.position, player.position) < attackDistance)
                        {
                            //Attack animation have event that call FightHit method to apply damage
                            animator.Play("Attack");
                        }
                    }
                }
                else
                {
                    notRoared = true;
                    Wandering();
                }
            }
            else
            {
                Death();
            }

        }
        
        public void Roar()
        {
            if (notRoared)
            {
                audioSource.PlayOneShot(roar);
                notRoared = false;
            }
        }

        private void Death()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = false;
            }

            GetComponent<Collider>().enabled = false;
            GetComponent<AIControl>().enabled = false;

            agent.enabled = false;
            animator.enabled = false;
            Destroy(this);
        }

        public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randomDirection = Random.insideUnitSphere * dist;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, dist, layermask))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        public void Wandering()
        {
            agent.speed = wanderingSpeed;

            timerW += Time.deltaTime;
            if (timerW >= wanderingTimer)
            {
                agent.SetDestination(RandomNavSphere(transform.position, wanderingRadius, 1));
                timerW = 0;
            }
            if (!agent.hasPath)
            {
                agent.SetDestination(RandomNavSphere(transform.position, wanderingRadius, 1));
            }
        }

        public void IsVisible(bool visible)
        {
            isVisible = visible;
        }

        public void FightHit()
        {
            if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
            {
                if(Vector3.Angle(transform.position, player.transform.position) < 20)
                audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
                playerCameraAnimator.Play("CameraKick");
                player.gameObject.GetComponent<PlayerStats>().ApplyDamage(Random.Range(1, maxDamage));
            }
        }

        private void Shout()
        {
            if (friendsList != null)
            {
                foreach (ZombieNPC friend in friendsList)
                {
                    if (friend != null && !friend.isWorried)
                    {
                        friend.isWorried = true;
                    }
                }
            }
        }

        IEnumerator CheckFriends()
        {
            for (; ; )
            {
                GetFriendsAround();
                yield return new WaitForSeconds(.5f);
            }
        }

        public void ApplyHit(int damage)
        {
            isWorried = true;
            Shout();
            health -= damage;
        }

        private void GetFriendsAround()
        {
            if (friendsList != null)
            {
                friendsList.Clear();
                friendsList.Capacity = 0;
            }

            var colliders = Physics.OverlapSphere(transform.position, 50f);
            foreach (Collider _collider in colliders)
            {
                if (_collider.gameObject.layer == this.gameObject.layer)
                {
                    if (friendsList != null && !friendsList.Contains(_collider.GetComponent<ZombieNPC>()))
                    {
                        if (_collider.gameObject != this.gameObject)
                        {
                            friendsList.Add(_collider.GetComponent<ZombieNPC>());
                        }
                    }
                }
            }
        }

        #region Character Controller

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            //animator.SetBool("LookAtTarget", lookAtTarget);
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            if (!lookAtTarget)
                m_TurnAmount = Mathf.Atan2(move.x, move.z);
            else
            {
                var lookPosition = player.transform.position - transform.position;
                lookPosition.y = 0;
                var rotation = Quaternion.LookRotation(lookPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);

            }


            m_ForwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            ScaleCapsuleForCrouching(crouch);

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }


        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
            //animator.SetFloat("Horizontal", move.x, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", m_Crouching);
            animator.SetBool("OnGround", m_IsGrounded);
            if (!m_IsGrounded)
            {
                animator.SetFloat("Jump", m_Rigidbody.linearVelocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            if (m_IsGrounded)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.magnitude > 0)
            {
                animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                animator.speed = 1;
            }
        }


        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.linearVelocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // jump!
                m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, m_JumpPower, m_Rigidbody.linearVelocity.z);
                m_IsGrounded = false;
                animator.applyRootMotion = false;
                m_GroundCheckDistance = 0.1f;
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }


        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.linearVelocity.y;
                m_Rigidbody.linearVelocity = v; // вызывает Warning
                /*
                Setting linear velocity of a kinematic body is not supported.
                UnityEngine.Rigidbody:set_linearVelocity(UnityEngine.Vector3)
                DarkTreeFPS.ZombieNPC:OnAnimatorMove()(at Assets / DarkTree FPS / Scripts / NPC / ZombieNPC.cs:437)
                */
            }
        }
        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                animator.applyRootMotion = true;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                animator.applyRootMotion = false;
            }
        }
        #endregion
    }
}
