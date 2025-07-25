using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TrackingSphereMove : MonoBehaviour
{

    // --- Public Fields ---
    public OVRSkeleton skeleton;
    public OVRSkeleton.BoneId boneIdOfInterest;

    // --- Private Fields ---
    private OVRBone boneOfInterest;
    private int boneIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (skeleton == null)
        {
            Debug.LogError("OVRSkeleton not assigned to TrackingSphereMove. Disabling script.");
            this.enabled = false;
            return;
        }

        boneOfInterest = skeleton.Bones.FirstOrDefault(b => b.Id == boneIdOfInterest);
        boneIndex = skeleton.Bones.IndexOf(boneOfInterest);
    }

    void Update()
    {
        if (!skeleton.IsInitialized || skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            return;
        }

        transform.position = skeleton.Bones[boneIndex].Transform.position;
        transform.rotation = skeleton.Bones[boneIndex].Transform.rotation;
    }
}
