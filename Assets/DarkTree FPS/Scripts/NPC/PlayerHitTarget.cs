using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTreeFPS;

public class PlayerHitTarget : MonoBehaviour
{
    public NPC npc;
    public bool isNPC;
    public float damageMultiplayer = 1f;
    [HideInInspector]
    public DTInventory.LootBox lootBox;
}
