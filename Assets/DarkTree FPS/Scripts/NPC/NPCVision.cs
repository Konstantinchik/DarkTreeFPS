/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here we looking for targets around NPC and calculate closest target

namespace DarkTreeFPS
{
    public class NPCVision : MonoBehaviour
    {
        [Tooltip("How often vision will check if player is in sight")]
        public float visibilityCheckInterval = 0.1f;
        [Tooltip("NPC field of fiev range")]
        public float FOV = 100;
        [Tooltip("How far NPC can look")]
        public float detectionRange = 30;

        private Transform player;
        public Collider playerCollider;

        private SphereCollider rangeCollider; // warning CS0169: The field 'NPCVision.rangeCollider' is never used

        public int targetFraction;

        public List<Collider> objectsInRange;
        public List<Collider> potentialTargets;
        public List<Collider> visibleTargets;

        public bool playerIsFriendly;

        public LayerMask layerMaskForVision;
        public LayerMask layerMaskForOverlap;
        
        private float timer;

        private NPC npc;

        public float detectionTimer;
        public float timeToDetect = 3f;
        public bool seePlayer;
        public bool playerDetected;

        public float hearDistance = 10f;

        private float detectionDistanceMultiplier;

        private void Start()
        {
            npc = GetComponentInParent<NPC>();
            player = GameObject.Find("Player").transform;
            playerCollider = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Collider>();
        }
        
        private void Update()
        {
            detectionDistanceMultiplier = 20 / Vector3.Distance(transform.position, player.position);

            if (Vector3.Distance(transform.position, player.position) < hearDistance && FPSController.CheckMovement() && !FPSController.crouch)
            {
                visibleTargets.Add(player.GetComponent<Collider>());
                potentialTargets.Add(player.GetComponent<Collider>());
                playerDetected = true;
            }
            
            if (seePlayer)
            {
                if (FPSController.crouch)
                    detectionTimer += Time.deltaTime * detectionDistanceMultiplier;
                else
                    detectionTimer += Time.deltaTime * 2 * detectionDistanceMultiplier;

                if (detectionTimer > timeToDetect + 0.1) detectionTimer = timeToDetect;
            }
            else
            {
                if(detectionTimer > 0 && !playerDetected)
                detectionTimer -= Time.deltaTime * 2;
            }

            if (detectionTimer > VisionUI.visionAmount && !playerDetected)
            {
                VisionUI.visionAmount = detectionTimer;
            }

            if (Time.time > timer + visibilityCheckInterval)
            {
                objectsInRange = new List<Collider>();
                visibleTargets = new List<Collider>();
                potentialTargets = new List<Collider>();

                var col = Physics.OverlapSphere(transform.position, detectionRange, layerMaskForOverlap);
                objectsInRange.AddRange(col);

                RaycastHit hit;
                
                foreach (var obj in objectsInRange)
                {
                    if (obj == null)
                        objectsInRange.Remove(obj);

                    if (obj.CompareTag("MainCamera") && !playerIsFriendly)
                    {
                        Vector3 direction = obj.transform.position - transform.position;
                        float angle = Vector3.Angle(direction, transform.forward);

                        Debug.DrawRay(transform.position, direction * 100);

                        if (angle < FOV * 0.5f)
                        {
                            if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMaskForVision))
                            {
                                if (hit.collider == obj)
                                {
                                    seePlayer = true;

                                    if ((!visibleTargets.Contains(obj) && detectionTimer > timeToDetect) || (!visibleTargets.Contains(obj) && playerDetected))
                                    {
                                        if (!PlayerStats.isPlayerDead)
                                        {
                                            playerDetected = true;
                                            visibleTargets.Add(obj);
                                            potentialTargets.Add(obj);
                                        }
                                    }
                                }
                                else
                                {
                                    seePlayer = false;

                                    if (visibleTargets.Contains(obj))
                                        visibleTargets.Remove(obj);
                                }
                            }
                            else
                            {
                                seePlayer = false;
                            }
                        }
                        else
                        {
                            seePlayer = false;
                        }
                    }

                    else if (obj.GetComponent<NPC>() != null && obj.GetComponent<NPC>().myFraction == targetFraction)
                    {
                        Vector3 direction = (obj.transform.position + Vector3.up) - transform.position;
                        float angle = Vector3.Angle(direction, transform.forward);

                        Debug.DrawRay(transform.position, direction * 100);

                        if (angle < FOV * 0.5f)
                        {
                            if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMaskForVision))
                            {

                                if (hit.collider == obj)
                                {
                                    if (!visibleTargets.Contains(obj))
                                    {
                                        visibleTargets.Add(obj);
                                        potentialTargets.Add(obj);
                                    }
                                }
                                else
                                {
                                    if (visibleTargets.Contains(obj))
                                        visibleTargets.Remove(obj);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (visibleTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                    }

                    if (obj.gameObject.layer == LayerMask.NameToLayer("Dead"))
                    {
                        if (visibleTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                        if (potentialTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                    }
                }
                timer = Time.time;
            }

            if(PlayerStats.isPlayerDead && potentialTargets.Contains(playerCollider))
            {
                potentialTargets.Remove(playerCollider);
            }
        }

        // Get target here!
        public Collider SelectBestTarget(bool visibleOnly)
        {
                // If we have targets in sight then we will select from them closest target
                if (visibleTargets != null)
                {
                    Collider closestTarget = null;
                    float bestDistance = 100f;

                    foreach (var target in visibleTargets)
                    {
                        if (target == null)
                            break;

                        var distance = Vector3.Distance(transform.position, target.transform.position);

                        if (distance < bestDistance)
                        {
                            closestTarget = target;
                            bestDistance = distance;
                        }
                    }

                    return closestTarget;
                }

            if (visibleOnly)
                return null;
            
            // If we have no visible targets, we will take one from visible before
            if(potentialTargets != null)
            {
                Collider closestTarget = null;
                float bestDistance = 100f;

                foreach (var target in potentialTargets)
                {
                    var distance = Vector3.Distance(transform.position, target.transform.position);

                    if (distance < bestDistance)
                    {
                        closestTarget = target;
                        bestDistance = distance;
                    }
                }

                return closestTarget;
            }

            // And finaly if we have not any targets then we just return null;
            return null;
        }
        
        public void TriggerPlayerShotVisibility()
        {
            if (Vector3.Distance(transform.position, player.position) < 80)
            {
                visibleTargets.Add(player.GetComponent<Collider>());
                potentialTargets.Add(player.GetComponent<Collider>());
                playerDetected = true;
            }
        }
    }
}