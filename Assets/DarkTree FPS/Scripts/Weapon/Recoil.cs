/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using System.Collections;

namespace DarkTreeFPS
{
    public class Recoil : MonoBehaviour
    {
        public float recoilReleaseSpeed = 2f;

        private void Update()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * recoilReleaseSpeed);
        }

        public void AddRecoil(Vector3 recoil)
        {
            transform.localRotation *= Quaternion.Euler(recoil);
            //transform.localRotation *= Quaternion.Euler(recoil + new Vector3(Random.Range(0,2), Random.Range(0, 2), Random.Range(0, 2)));
        }
    }
}