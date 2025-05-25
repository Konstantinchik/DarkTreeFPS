using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIKLite : MonoBehaviour {

    public Transform[] bones;
    public Transform aimTransform;
    public Vector3 axis;
    public Transform target;
    public int iterations = 4;

    private void LateUpdate()
    {
        if (target == null)
            return;

        float w = 1f / (float)bones.Length;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                var lastBone = bones[bones.Length - 1];
                Quaternion f = Quaternion.FromToRotation(aimTransform.rotation * axis, target.position - aimTransform.position);
                f = Quaternion.Slerp(Quaternion.identity, f, w * (i + 1));
                bones[i].rotation = f * bones[i].rotation;
            }
        }
    }
}
