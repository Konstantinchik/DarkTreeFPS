using UnityEngine;

public class HeliTrigger : MonoBehaviour
{
    public Animator heliAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            heliAnimator.enabled = true;
        }
    }
}
