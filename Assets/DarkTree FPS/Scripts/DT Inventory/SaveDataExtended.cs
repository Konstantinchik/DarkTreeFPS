using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using DarkTreeFPS;

namespace DTInventory
{
    public class InventoryData
    {
        public string[] itemNames;
        public int[] stackSize;
        public Vector2[] itemGridPos;
        public int activeWeaponIndex;

        public InventoryData(string[] itemNames, int[] stackSize, Vector2[] itemGridPos, int activeWeaponIndex)
        {
            this.itemNames = itemNames;
            this.stackSize = stackSize;
            this.itemGridPos = itemGridPos;
            this.activeWeaponIndex = activeWeaponIndex;
        }
    }

    public class CharactersData
    {
        //NPC

        public string[] npcName;
        public Vector3[] npcPos;
        public Quaternion[] npcRot;
        public Vector3[] npcCurrentTarget;
        public Vector3[] npcLookAtTarget;

        //Zombies

        public Vector3[] zombiePos;
        public Quaternion[] zombieRot;
        public bool[] zombieIsWorried;
    }

    public class LevelData
    {
        public Vector3[] itemPos;
        public Quaternion[] itemRot;
        public string[] itemName;
        public int[] itemStackSize;
    }

    public class LootBoxData
    {
        public string lootBoxSceneNames;
        public string[] itemNames;
        public string[] stackSize;
    }

    public class PlayerStatsData
    {
        public int health;
        public bool useConsumeSystem;
        public int hydratation;
        public float hydratationSubstractionRate;
        public int thirstDamage;
        public float hydratationTimer;
        public int satiety;
        public float satietySubstractionRate;
        public int hungerDamage;
        public float satietyTimer;

        public Vector3 playerPosition;
        public Quaternion playerRotation;

        public Quaternion camRotation;

        public Vector2 targetDirection;
        public Vector2 mouseAbsolute;
        public Vector2 smoothMouse;

        public PlayerStatsData(int health, bool useConsumeSystem, int hydratation, float hydratationSubstractionRate, int thirstDamage,
                               float hydratationTimer, int satiety, float satietySubstractionRate, int hungerDamage, float satietyTimer,
                               Vector3 playerPosition, Quaternion playerRotation, Quaternion camRotation, Vector2 targetDirection, Vector2 mouseAbsolute, Vector2 smoothMouse)
        {
            this.health = health;
            this.useConsumeSystem = useConsumeSystem;
            this.hydratation = hydratation;
            this.hydratationSubstractionRate = hydratationSubstractionRate;
            this.thirstDamage = thirstDamage;
            this.hydratationTimer = hydratationTimer;
            this.satiety = satiety;
            this.satietySubstractionRate = satietySubstractionRate;
            this.hungerDamage = hungerDamage;
            this.satietyTimer = satietyTimer;
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.camRotation = camRotation;
            this.targetDirection = targetDirection;
            this.mouseAbsolute = mouseAbsolute;
            this.smoothMouse = smoothMouse;
        }
    }
}