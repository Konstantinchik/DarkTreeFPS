/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DarkTreeFPS
{
    /// <summary>
    /// It's very simple class so I think that there is no need in deep explanation of it
    /// When health less than or equal to zero -> Death() || if respawn -> Respawn()
    /// Death and Respawn methods works in same way but in oposite direction
    /// </summary>

    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        public bool isGod = false;                          // Надо привязать god mode из консоли
        [Tooltip("Player's health")]
        public int health = 100;
        [Tooltip("UI element to draw health as number")]
        public Text healthUIText;
        [Tooltip("UI element to draw health as slider")]
        public Slider healthUISlider;
        public Slider staminaUISlider;
        public Text staminaUIText;
        
        public Slider hungerSlider;
        public Text hungerUIText;

        public Slider thirstSlider;
        public Text thirstUIText;

        public float minFailingDamage = 10;

        [Header("Damage effect")]
        [Tooltip("UI Image with fullscreen hit fx")]
        public Image damageScreenFX;
        [Tooltip("UI Image color to change on hit")]
        public Color damageScreenColor;
        [Tooltip("UI Image fade speed after hit")]
        public float damageScreenFadeSpeed = 1.4f;

        [Header("Consume stats")]
        public bool useConsumeSystem = true;          // Вкл / Выкл потребление
        
        public int hydration = 100;
        public float hydrationSubstractionRate = 3f;
        public int thirstDamage = 1;
        [HideInInspector]
        public float hydrationTimer;


        public int satiety = 100;
        public float satietySubstractionRate = 5f;
        public int hungerDamage = 1;
        [HideInInspector]
        public float satietyTimer;

        public Text playerStats;

        public float stamina = 100;
        public float staminaRestoreSpeed = 2;
        public float staminaDecreaseSpeed = 5;

        private Color damageScreenColor_temp;

        public static bool isPlayerDead = false;

        [HideInInspector]
        public Vector3 playerPosition;
        [HideInInspector]
        public Quaternion playerRotation;

        private AudioSource audioSource;
        public AudioClip[] painClips;
        public AudioClip breathingSound;

        private GameObject playerBody;
        private InputManager inputManager;

        #region utility objects
        private Rigidbody playerRigidbody;
        private FPSController controller;
        private CapsuleCollider playerCollider;
        private Sway sway;

        private Transform cameraHolder;

        private WeaponManager weaponManager;
        //Don't create any rigidbody here
        private Rigidbody rigidbody_temp;
        #endregion
        
        private void Start()
        {
            cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();

            isPlayerDead = false;

            audioSource = GetComponent<AudioSource>();

            playerRigidbody = GetComponent<Rigidbody>();
            controller = GetComponent<FPSController>();
            playerCollider = GetComponent<CapsuleCollider>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
            sway = FindFirstObjectByType<Sway>();

            inputManager = FindFirstObjectByType<InputManager>();

            if(!InputManager.useMobileInput)
            playerBody = FindFirstObjectByType<Body>().gameObject;
        }
        
        void Update()
        {
            if (isPlayerDead)
            {
                weaponManager.gameObject.SetActive(false);
            }

            if (health == 0 && !isPlayerDead && !isGod)
            {
                PlayerDeath();
            }

            if (health < 0)
            {
                health = 0;
            }

            if(health >  100)
            {
                health = 100;
            }

            WritePlayerTransform();
            ConsumableManager(useConsumeSystem);
            DrawHealthStats();
            DrawPlayerStats();
        }
        
        public void ConsumableManager(bool useSystem)
        {
            if (!useSystem)
                return;

            if (Time.time > satietyTimer + satietySubstractionRate)
            {
                if (satiety <= 0)
                {
                    satiety = 0;
                    health -= hungerDamage;
                }

                satiety -= 1;
                satietyTimer = Time.time;
                
            }

            if (Time.time > hydrationTimer + hydrationSubstractionRate)
            {
                if (hydration <= 0)
                {
                    hydration = 0;
                    health -= thirstDamage;
                }
                hydration -= 1;
                hydrationTimer = Time.time;
            }

            if(inputManager.IsRunning() && stamina > 0)
            {
                stamina -= Time.deltaTime * staminaDecreaseSpeed;
            }

            if(!inputManager.IsRunning() && stamina < 100)
            {
                stamina += Time.deltaTime * staminaRestoreSpeed;
            }

            if(hydration > 100)
            {
                hydration = 100;
            }
            if(satiety > 100)
            {
                satiety = 100;
            }
        }

        public void DrawPlayerStats()
        {
            if(playerStats != null)
                playerStats.text = string.Format("--- Player statistic ---\n\n\n - Health: {0}\n\n - Hydratation: {1}\n\n - Satiety: {2}\n\n", health, hydration, satiety);
        }

        public void ApplyDamage(int damage)
        {
            if (isPlayerDead)
                return;

            if (damage > 0)
            {
                if(audioSource != null && painClips.Length > 0)
                {
                    audioSource.PlayOneShot(painClips[Random.Range(0, painClips.Length)]);
                }

                health -= damage;
                damageScreenFX.color = damageScreenColor;
                damageScreenColor_temp = damageScreenColor;
                StartCoroutine(HitFX());
            }
        }

        public void AddSatiety(int points)
        {
            satiety += points;
        }

        public void AddHydration(int points)
        {
            hydration += points;
        }

        public void AddHealth(int hp)
        {
            health += hp;
        }

        IEnumerator HitFX()
        {
            while (damageScreenFX.color.a > 0)
            {
                damageScreenColor_temp = new Color(damageScreenColor_temp.r, damageScreenColor_temp.g, damageScreenColor_temp.b, damageScreenColor_temp.a -= damageScreenFadeSpeed * Time.deltaTime);
                damageScreenFX.color = damageScreenColor_temp;

                yield return new WaitForEndOfFrame();
            }
        }

        public void PlayerDeath()
        {
            if (!isPlayerDead)
            {
                sway.enabled = false;
                var leanController = FindFirstObjectByType<Lean>().enabled = false;
                controller.enabled = false;
                
                if(playerBody != null && !InputManager.useMobileInput)
                {
                    playerBody.SetActive(false);
                }

                if (!rigidbody_temp)
                {
                    rigidbody_temp = cameraHolder.gameObject.AddComponent<Rigidbody>();
                    rigidbody_temp.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    rigidbody_temp.mass = 10;
                }

                Camera.main.GetComponent<Animator>().enabled = false;
                Destroy(Camera.main.GetComponent<Rigidbody>());

                cameraHolder.parent = null;
                cameraHolder.GetComponent<BoxCollider>().enabled = true;

                rigidbody_temp.AddForce(cameraHolder.transform.forward * 5, ForceMode.VelocityChange);
                rigidbody_temp.constraints = RigidbodyConstraints.FreezeRotation;
                

                weaponManager.HideWeaponOnDeath();
                
                controller.lockCursor = false;

                isPlayerDead = true;
            }
            else
                return;
        }

        public void RespawnPlayer()
        {
            isPlayerDead = false;

            FindFirstObjectByType<FPSController>().mainCameraAnimator.gameObject.SetActive(true);

            sway.enabled = true;
            var leanController = FindFirstObjectByType<Lean>().enabled = true;
            controller.enabled = true;
            
            if (playerBody != null && !InputManager.useMobileInput)
            {
                playerBody.SetActive(true);
            }
            
            controller.lockCursor = false;
        }
        
        void WritePlayerTransform()
        {
            playerPosition = gameObject.transform.position;
            playerRotation = gameObject.transform.rotation;
        }

        void DrawHealthStats()
        {
            if (healthUIText != null)
                healthUIText.text = health.ToString();

            if (healthUISlider != null)
                healthUISlider.value = health;

            if (hungerUIText != null)
                hungerUIText.text = satiety.ToString();

            if (hungerSlider != null) hungerSlider.value = satiety;

            if (thirstUIText != null)
                thirstUIText.text = hydration.ToString();

            if (staminaUISlider != null)
                staminaUISlider.value = (int)stamina;

            if (staminaUIText != null)
                staminaUIText.text = ((int)stamina).ToString();

            if (thirstSlider != null) thirstSlider.value = hydration;


        }
    }

}