/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.UI;
using DTInventory;

namespace DarkTreeFPS
{
    public class UseObjects : MonoBehaviour
    {
        [Tooltip("The distance within which you can pick up item")]
        public float distance = 1.5f;
        
        private GameObject use;
        private GameObject useCursor;
        private Text useText;

        private InputManager input;
        private DTInventory.DTInventory inventory;

        private Button useButton;

        public static bool useState;

        private WeaponManager weaponManager;
        private SoundManager soundManager;
        private PickupItem pickupItem;

        private void Start()
        {
            useCursor = GameObject.Find("UseCursor");
            if (useCursor != null)
            {
                useText = useCursor.GetComponentInChildren<Text>();
                useCursor.SetActive(false);
            }

            inventory = FindFirstObjectByType<DTInventory.DTInventory>();
            input = FindFirstObjectByType<InputManager>();

            weaponManager = FindFirstObjectByType<WeaponManager>();

            soundManager = FindFirstObjectByType<SoundManager>();

            pickupItem = FindFirstObjectByType<PickupItem>();

            if (InputManager.useMobileInput)
            {
                useButton = GameObject.Find("UseButton").GetComponent<Button>();
                useButton.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            Pickup();

            if(InventoryManager.showInventory && InputManager.useMobileInput)
            {
                useButton.gameObject.SetActive(false);
            }
        }

        public void Pickup()
        {
            RaycastHit hit;

            //Hit an object within pickup distance
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                if (hit.collider.tag == "Item")
                {
                    useState = true;
                    //Get an item which we want to pickup
                    use = hit.collider.gameObject;
                    useCursor.SetActive(true);

                    if (InputManager.useMobileInput)
                        useButton.gameObject.SetActive(true);

                    if (use.GetComponent<Item>())
                    {
                        useText.text = use.GetComponent<Item>().title;

                        if (!InputManager.useMobileInput)
                        {
                            if (Input.GetKeyDown(input.Use))
                            {
                                soundManager.Pickup();
                                inventory.AddItem(use.GetComponent<Item>());
                                use = null;
                            }
                        }
                        if(InputManager.useMobileInput)
                        {
                            var item = use.GetComponent<Item>();
                            useButton.onClick.RemoveAllListeners();
                            useButton.onClick.AddListener(() => { inventory.AddItem(item); });
                            use = null;
                            return;
                        }
                            
                    }

                }else if(hit.collider.tag == "LootBox")
                {
                    if (InputManager.useMobileInput)
                        useButton.gameObject.SetActive(true);

                    useCursor.SetActive(true);
                    useText.text = "Inspect";

                    if (InputManager.useMobileInput)
                    {
                        useButton.onClick.RemoveAllListeners();
                        useButton.onClick.AddListener(() => { pickupItem.InspectLootBox(hit.collider.GetComponent<LootBox>()); });
                        use = null;
                        return;
                    }
                }
                else
                {
                    useState = false;
                    //Clear use object if there is no an object with "Item" tag
                    use = null;
                    useCursor.SetActive(false);

                    if(InputManager.useMobileInput)
                        useButton.gameObject.SetActive(false);

                    useText.text = "";
                }
            }
            else
            {
                useState = false;

                if(useCursor != null)
                useCursor.SetActive(false);

                if (InputManager.useMobileInput)
                    useButton.gameObject.SetActive(false);

                if (useText != null)
                {
                    useText.text = "";
                }
            }
        }
    }
}
