using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTreeFPS
{
    public class RadioactiveZone : MonoBehaviour
    {
        public GameObject heigerSource;
        public GameObject heigerSourceHi;

        public Transform player;

        public float lowRadDistance = 15f;
        public float highRadDistance = 5f;
        
        PlayerStats playerStats;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        public float damageLow = 0.5f;
        public float damageHi = 2f;

        public float damage = 1;

        public float timeToDamage;
        private float timer;

        private void Update()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            
                if(Time.time  > timer + timeToDamage)
                {
                    playerStats.health -= (int)damage;
                    timer = Time.time;
                }
        }

        private void LateUpdate()
        {
            if (player != null)
            {
                if (Vector3.Distance(transform.position, player.position) < highRadDistance)
                {
                    heigerSourceHi.SetActive(true);
                    heigerSource.SetActive(false);
                    damage = damageHi;
                }
                else if (Vector3.Distance(transform.position, player.position) < lowRadDistance || Vector3.Distance(transform.position, player.position) > highRadDistance)
                {
                    heigerSource.SetActive(true);
                    heigerSourceHi.SetActive(false);
                    damage = damageLow;
                }
                if (Vector3.Distance(transform.position, player.position) > lowRadDistance)
                {
                    heigerSource.SetActive(false);
                    heigerSourceHi.SetActive(false);
                    damage = 0;
                }
            }
        }
    }
}
