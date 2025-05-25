using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRay : MonoBehaviour
{
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 100);
    }
}
