using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool occupied = false;
    public Collider m_collider;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }
}
