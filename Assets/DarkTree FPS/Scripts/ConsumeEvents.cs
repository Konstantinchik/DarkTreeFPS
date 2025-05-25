/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.Events;
using DTInventory;

/// <summary>
/// This class automaticly creates listener for item use event.
/// When we will consume some item we will use reciver method from that script
/// </summary>

namespace DarkTreeFPS {

    public class ConsumeEvents : MonoBehaviour
    {
        public enum ConsumableEvents { addHealth, addSatiety, addHydration }

        [Header("What reference should I add?")]
        public ConsumableEvents m_Event;

        public int pointsToAdd;

        private PlayerStats playerStats;
        private Item item;

        public float actionWaitTimer = 0;
        public string actionWaitText;
        public AudioClip actionWaitSFX;

        UnityAction addHealth, addSatiety, addHydration;

        ActionWait actionWait;

        private void OnEnable()
        {
            actionWait = FindFirstObjectByType<ActionWait>();
            playerStats = FindFirstObjectByType<PlayerStats>();
            item = GetComponent<Item>();

            addHealth += AddHealth;
            addHydration += AddHydration;
            addSatiety += AddSatiety;

            switch (m_Event)
            {
                case ConsumableEvents.addHealth:
                    item.onUseEvent.AddListener(addHealth);
                    break;
                case ConsumableEvents.addHydration:
                    item.onUseEvent.AddListener(addHydration);
                    break;
                case ConsumableEvents.addSatiety:
                    item.onUseEvent.AddListener(addSatiety);
                    break;
            }
        }

        public void ActionWait()
        {
            actionWait.actionText = actionWaitText;
            actionWait.audioSource.PlayOneShot(actionWaitSFX);
            if (actionWaitTimer > 0)
                actionWait.actionTimer = actionWaitTimer;
        }

        public void AddHealth()
        {
            playerStats.AddHealth(pointsToAdd);
            ActionWait();
        }

        public void AddSatiety()
        {
            playerStats.AddSatiety(pointsToAdd);
            ActionWait();
        }

        public void AddHydration()
        {
            playerStats.AddHydration(pointsToAdd);
            ActionWait();
        }


    }
}
