using System.Collections.Generic;
using UnityEngine;
using DarkTreeFPS;

public class CoverList : MonoBehaviour
{
    public List<Cover> covers;

    public int maxCoverFindRange = 10;

    public static List<NPC> sceneActiveNPC;
    
    private float timer; // warning CS0169: The field 'CoverList.timer' is never used
    private float npcListRefreshTimer = 0.2f; // warning CS0414: The field 'CoverList.npcListRefreshTimer' is assigned but its value is never used

    private void Awake()
    {
        sceneActiveNPC = new List<NPC>();
    }

    public void NotifyNPCAboutShot(Vector3 playerPosition, float weaponShotNPCDetectionDistance)
    {
        foreach(var npc in sceneActiveNPC)
        {
            print("Notify npc call: Distance is: " + Vector3.Distance(npc.transform.position, playerPosition)+", and detection distance is: "+weaponShotNPCDetectionDistance);

            if(npc != null && Vector3.Distance(npc.transform.position, playerPosition) < weaponShotNPCDetectionDistance)
            {
                npc.ListenToPlayerShot();
            }
        }
    }

    private void Start()
    {
        covers.AddRange(FindObjectsOfType<Cover>());
    }
    
    public Cover FindClosestCover(Vector3 myPos, Vector3 enemyPos)
    {
        if (covers == null) return null;
        if (enemyPos == null) return null;

        Cover cover = null;

        var bestDistance = 1000f;

        RaycastHit hit;

        foreach (var _cover in covers)
        {
            var distance = Vector3.Distance(myPos, _cover.transform.position);
            var direction = _cover.transform.position - enemyPos;

            if (Physics.Raycast(enemyPos, direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.name != _cover.m_collider.name)
                {
                    if (distance < bestDistance && Vector3.Distance(_cover.transform.position, enemyPos) > 5)
                    {
                        if (!_cover.occupied)
                        {
                                cover = _cover;
                                bestDistance = distance;
                            
                        }
                    }
                }
            }
        }

        return cover;
    }
}
