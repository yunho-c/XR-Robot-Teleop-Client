#region Assembly Oculus.VR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// location unknown
// Decompiled with ICSharpCode.Decompiler 9.1.0.7988
#endregion

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class OVRSkeleton : MonoBehaviour
{
    public interface IOVRSkeletonDataProvider
    {
        bool enabled { get; }

        SkeletonType GetSkeletonType();

        SkeletonPoseData GetSkeletonPoseData();
    }

    public struct SkeletonPoseData
    {
        public OVRPlugin.Posef RootPose { get; set; }

        public float RootScale { get; set; }

        public OVRPlugin.Quatf[] BoneRotations { get; set; }

        public bool IsDataValid { get; set; }

        public bool IsDataHighConfidence { get; set; }

        public OVRPlugin.Vector3f[] BoneTranslations { get; set; }

        public int SkeletonChangedCount { get; set; }
    }

    public enum SkeletonType
    {
        None = -1,
        [InspectorName("OVR Hand (Left)")]
        HandLeft,
        [InspectorName("OVR Hand (Right)")]
        HandRight,
        Body,
        FullBody,
        [InspectorName("OpenXR Hand (Left)")]
        XRHandLeft,
        [InspectorName("OpenXR Hand (Right)")]
        XRHandRight
    }

    public enum BoneId
    {
        Invalid = -1,
        Hand_Start = 0,
        Hand_WristRoot = 0,
        Hand_ForearmStub = 1,
        Hand_Thumb0 = 2,
        Hand_Thumb1 = 3,
        Hand_Thumb2 = 4,
        Hand_Thumb3 = 5,
        Hand_Index1 = 6,
        Hand_Index2 = 7,
        Hand_Index3 = 8,
        Hand_Middle1 = 9,
        Hand_Middle2 = 10,
        Hand_Middle3 = 11,
        Hand_Ring1 = 12,
        Hand_Ring2 = 13,
        Hand_Ring3 = 14,
        Hand_Pinky0 = 15,
        Hand_Pinky1 = 16,
        Hand_Pinky2 = 17,
        Hand_Pinky3 = 18,
        Hand_MaxSkinnable = 19,
        Hand_ThumbTip = 19,
        Hand_IndexTip = 20,
        Hand_MiddleTip = 21,
        Hand_RingTip = 22,
        Hand_PinkyTip = 23,
        Hand_End = 24,
        XRHand_Start = 0,
        XRHand_Palm = 0,
        XRHand_Wrist = 1,
        XRHand_ThumbMetacarpal = 2,
        XRHand_ThumbProximal = 3,
        XRHand_ThumbDistal = 4,
        XRHand_ThumbTip = 5,
        XRHand_IndexMetacarpal = 6,
        XRHand_IndexProximal = 7,
        XRHand_IndexIntermediate = 8,
        XRHand_IndexDistal = 9,
        XRHand_IndexTip = 10,
        XRHand_MiddleMetacarpal = 11,
        XRHand_MiddleProximal = 12,
        XRHand_MiddleIntermediate = 13,
        XRHand_MiddleDistal = 14,
        XRHand_MiddleTip = 15,
        XRHand_RingMetacarpal = 16,
        XRHand_RingProximal = 17,
        XRHand_RingIntermediate = 18,
        XRHand_RingDistal = 19,
        XRHand_RingTip = 20,
        XRHand_LittleMetacarpal = 21,
        XRHand_LittleProximal = 22,
        XRHand_LittleIntermediate = 23,
        XRHand_LittleDistal = 24,
        XRHand_LittleTip = 25,
        XRHand_Max = 26,
        XRHand_End = 26,
        Body_Start = 0,
        Body_Root = 0,
        Body_Hips = 1,
        Body_SpineLower = 2,
        Body_SpineMiddle = 3,
        Body_SpineUpper = 4,
        Body_Chest = 5,
        Body_Neck = 6,
        Body_Head = 7,
        Body_LeftShoulder = 8,
        Body_LeftScapula = 9,
        Body_LeftArmUpper = 10,
        Body_LeftArmLower = 11,
        Body_LeftHandWristTwist = 12,
        Body_RightShoulder = 13,
        Body_RightScapula = 14,
        Body_RightArmUpper = 15,
        Body_RightArmLower = 16,
        Body_RightHandWristTwist = 17,
        Body_LeftHandPalm = 18,
        Body_LeftHandWrist = 19,
        Body_LeftHandThumbMetacarpal = 20,
        Body_LeftHandThumbProximal = 21,
        Body_LeftHandThumbDistal = 22,
        Body_LeftHandThumbTip = 23,
        Body_LeftHandIndexMetacarpal = 24,
        Body_LeftHandIndexProximal = 25,
        Body_LeftHandIndexIntermediate = 26,
        Body_LeftHandIndexDistal = 27,
        Body_LeftHandIndexTip = 28,
        Body_LeftHandMiddleMetacarpal = 29,
        Body_LeftHandMiddleProximal = 30,
        Body_LeftHandMiddleIntermediate = 31,
        Body_LeftHandMiddleDistal = 32,
        Body_LeftHandMiddleTip = 33,
        Body_LeftHandRingMetacarpal = 34,
        Body_LeftHandRingProximal = 35,
        Body_LeftHandRingIntermediate = 36,
        Body_LeftHandRingDistal = 37,
        Body_LeftHandRingTip = 38,
        Body_LeftHandLittleMetacarpal = 39,
        Body_LeftHandLittleProximal = 40,
        Body_LeftHandLittleIntermediate = 41,
        Body_LeftHandLittleDistal = 42,
        Body_LeftHandLittleTip = 43,
        Body_RightHandPalm = 44,
        Body_RightHandWrist = 45,
        Body_RightHandThumbMetacarpal = 46,
        Body_RightHandThumbProximal = 47,
        Body_RightHandThumbDistal = 48,
        Body_RightHandThumbTip = 49,
        Body_RightHandIndexMetacarpal = 50,
        Body_RightHandIndexProximal = 51,
        Body_RightHandIndexIntermediate = 52,
        Body_RightHandIndexDistal = 53,
        Body_RightHandIndexTip = 54,
        Body_RightHandMiddleMetacarpal = 55,
        Body_RightHandMiddleProximal = 56,
        Body_RightHandMiddleIntermediate = 57,
        Body_RightHandMiddleDistal = 58,
        Body_RightHandMiddleTip = 59,
        Body_RightHandRingMetacarpal = 60,
        Body_RightHandRingProximal = 61,
        Body_RightHandRingIntermediate = 62,
        Body_RightHandRingDistal = 63,
        Body_RightHandRingTip = 64,
        Body_RightHandLittleMetacarpal = 65,
        Body_RightHandLittleProximal = 66,
        Body_RightHandLittleIntermediate = 67,
        Body_RightHandLittleDistal = 68,
        Body_RightHandLittleTip = 69,
        Body_End = 70,
        FullBody_Start = 0,
        FullBody_Root = 0,
        FullBody_Hips = 1,
        FullBody_SpineLower = 2,
        FullBody_SpineMiddle = 3,
        FullBody_SpineUpper = 4,
        FullBody_Chest = 5,
        FullBody_Neck = 6,
        FullBody_Head = 7,
        FullBody_LeftShoulder = 8,
        FullBody_LeftScapula = 9,
        FullBody_LeftArmUpper = 10,
        FullBody_LeftArmLower = 11,
        FullBody_LeftHandWristTwist = 12,
        FullBody_RightShoulder = 13,
        FullBody_RightScapula = 14,
        FullBody_RightArmUpper = 15,
        FullBody_RightArmLower = 16,
        FullBody_RightHandWristTwist = 17,
        FullBody_LeftHandPalm = 18,
        FullBody_LeftHandWrist = 19,
        FullBody_LeftHandThumbMetacarpal = 20,
        FullBody_LeftHandThumbProximal = 21,
        FullBody_LeftHandThumbDistal = 22,
        FullBody_LeftHandThumbTip = 23,
        FullBody_LeftHandIndexMetacarpal = 24,
        FullBody_LeftHandIndexProximal = 25,
        FullBody_LeftHandIndexIntermediate = 26,
        FullBody_LeftHandIndexDistal = 27,
        FullBody_LeftHandIndexTip = 28,
        FullBody_LeftHandMiddleMetacarpal = 29,
        FullBody_LeftHandMiddleProximal = 30,
        FullBody_LeftHandMiddleIntermediate = 31,
        FullBody_LeftHandMiddleDistal = 32,
        FullBody_LeftHandMiddleTip = 33,
        FullBody_LeftHandRingMetacarpal = 34,
        FullBody_LeftHandRingProximal = 35,
        FullBody_LeftHandRingIntermediate = 36,
        FullBody_LeftHandRingDistal = 37,
        FullBody_LeftHandRingTip = 38,
        FullBody_LeftHandLittleMetacarpal = 39,
        FullBody_LeftHandLittleProximal = 40,
        FullBody_LeftHandLittleIntermediate = 41,
        FullBody_LeftHandLittleDistal = 42,
        FullBody_LeftHandLittleTip = 43,
        FullBody_RightHandPalm = 44,
        FullBody_RightHandWrist = 45,
        FullBody_RightHandThumbMetacarpal = 46,
        FullBody_RightHandThumbProximal = 47,
        FullBody_RightHandThumbDistal = 48,
        FullBody_RightHandThumbTip = 49,
        FullBody_RightHandIndexMetacarpal = 50,
        FullBody_RightHandIndexProximal = 51,
        FullBody_RightHandIndexIntermediate = 52,
        FullBody_RightHandIndexDistal = 53,
        FullBody_RightHandIndexTip = 54,
        FullBody_RightHandMiddleMetacarpal = 55,
        FullBody_RightHandMiddleProximal = 56,
        FullBody_RightHandMiddleIntermediate = 57,
        FullBody_RightHandMiddleDistal = 58,
        FullBody_RightHandMiddleTip = 59,
        FullBody_RightHandRingMetacarpal = 60,
        FullBody_RightHandRingProximal = 61,
        FullBody_RightHandRingIntermediate = 62,
        FullBody_RightHandRingDistal = 63,
        FullBody_RightHandRingTip = 64,
        FullBody_RightHandLittleMetacarpal = 65,
        FullBody_RightHandLittleProximal = 66,
        FullBody_RightHandLittleIntermediate = 67,
        FullBody_RightHandLittleDistal = 68,
        FullBody_RightHandLittleTip = 69,
        FullBody_LeftUpperLeg = 70,
        FullBody_LeftLowerLeg = 71,
        FullBody_LeftFootAnkleTwist = 72,
        FullBody_LeftFootAnkle = 73,
        FullBody_LeftFootSubtalar = 74,
        FullBody_LeftFootTransverse = 75,
        FullBody_LeftFootBall = 76,
        FullBody_RightUpperLeg = 77,
        FullBody_RightLowerLeg = 78,
        FullBody_RightFootAnkleTwist = 79,
        FullBody_RightFootAnkle = 80,
        FullBody_RightFootSubtalar = 81,
        FullBody_RightFootTransverse = 82,
        FullBody_RightFootBall = 83,
        FullBody_End = 84,
        Max = 84
    }

    [SerializeField]
    protected SkeletonType _skeletonType = SkeletonType.None;

    [SerializeField]
    private IOVRSkeletonDataProvider _dataProvider;

    [SerializeField]
    private bool _updateRootPose;

    [SerializeField]
    private bool _updateRootScale;

    [SerializeField]
    private bool _enablePhysicsCapsules;

    [SerializeField]
    private bool _applyBoneTranslations = true;

    private GameObject _bonesGO;

    private GameObject _bindPosesGO;

    private GameObject _capsulesGO;

    protected List<OVRBone> _bones;

    private List<OVRBone> _bindPoses;

    private List<OVRBoneCapsule> _capsules;

    protected OVRPlugin.Skeleton2 _skeleton;

    private readonly Quaternion wristFixupRotation = new Quaternion(0f, 1f, 0f, 0f);

    public bool IsInitialized { get; private set; }

    public bool IsDataValid { get; private set; }

    public bool IsDataHighConfidence { get; private set; }

    public IList<OVRBone> Bones { get; protected set; }

    public IList<OVRBone> BindPoses { get; private set; }

    public IList<OVRBoneCapsule> Capsules { get; private set; }

    public int SkeletonChangedCount { get; private set; }

    public SkeletonType GetSkeletonType()
    {
        return _skeletonType;
    }

    internal virtual void SetSkeletonType(SkeletonType type)
    {
        bool num = IsInitialized && type != _skeletonType;
        _skeletonType = type;
        if (num)
        {
            Initialize();
            OVRMeshRenderer component = GetComponent<OVRMeshRenderer>();
            if (component != null)
            {
                component.ForceRebind();
            }
        }
    }

    internal OVRPlugin.BodyJointSet GetRequiredBodyJointSet()
    {
        return _skeletonType switch
        {
            SkeletonType.Body => OVRPlugin.BodyJointSet.UpperBody,
            SkeletonType.FullBody => OVRPlugin.BodyJointSet.FullBody,
            _ => OVRPlugin.BodyJointSet.None,
        };
    }

    public bool IsValidBone(BoneId bone)
    {
        return OVRPlugin.IsValidBone((OVRPlugin.BoneId)bone, (OVRPlugin.SkeletonType)_skeletonType);
    }

    protected virtual void Awake()
    {
        if (_dataProvider == null)
        {
            IOVRSkeletonDataProvider iOVRSkeletonDataProvider = SearchSkeletonDataProvider();
            if (iOVRSkeletonDataProvider != null)
            {
                _dataProvider = iOVRSkeletonDataProvider;
                if (_dataProvider is MonoBehaviour monoBehaviour)
                {
                    Debug.Log("Found IOVRSkeletonDataProvider reference in " + monoBehaviour.name + " due to unassigned field.");
                }
            }
        }

        _bones = new List<OVRBone>();
        Bones = _bones.AsReadOnly();
        _bindPoses = new List<OVRBone>();
        BindPoses = _bindPoses.AsReadOnly();
        _capsules = new List<OVRBoneCapsule>();
        Capsules = _capsules.AsReadOnly();
    }

    internal IOVRSkeletonDataProvider SearchSkeletonDataProvider()
    {
        IOVRSkeletonDataProvider[] componentsInParent = base.gameObject.GetComponentsInParent<IOVRSkeletonDataProvider>(includeInactive: true);
        foreach (IOVRSkeletonDataProvider iOVRSkeletonDataProvider in componentsInParent)
        {
            if (iOVRSkeletonDataProvider.GetSkeletonType() == _skeletonType)
            {
                return iOVRSkeletonDataProvider;
            }
        }

        return null;
    }

    protected virtual void Start()
    {
        if (_dataProvider == null && _skeletonType == SkeletonType.Body)
        {
            Debug.LogWarning("OVRSkeleton and its subclasses requires OVRBody to function.");
        }

        if (ShouldInitialize())
        {
            Initialize();
        }
    }

    private bool ShouldInitialize()
    {
        if (IsInitialized)
        {
            return false;
        }

        if (_dataProvider != null && !_dataProvider.enabled)
        {
            return false;
        }

        if (_skeletonType == SkeletonType.None)
        {
            return false;
        }

        IsHandSkeleton(_skeletonType);
        return true;
    }

    private void Initialize()
    {
        if (OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)_skeletonType, ref _skeleton))
        {
            InitializeBones();
            InitializeBindPose();
            InitializeCapsules();
            IsInitialized = true;
        }
    }

    protected virtual Transform GetBoneTransform(BoneId boneId)
    {
        return null;
    }

    protected virtual void InitializeBones()
    {
        bool flag = _skeletonType.IsOVRHandSkeleton();
        if (!_bonesGO)
        {
            _bonesGO = new GameObject("Bones");
            _bonesGO.transform.SetParent(base.transform, worldPositionStays: false);
            _bonesGO.transform.localPosition = Vector3.zero;
            _bonesGO.transform.localRotation = Quaternion.identity;
        }

        if (_bones == null || _bones.Count != _skeleton.NumBones)
        {
            if (_bones != null)
            {
                for (int i = 0; i < _bones.Count; i++)
                {
                    _bones[i].Dispose();
                }

                _bones.Clear();
            }

            _bones = new List<OVRBone>(new OVRBone[_skeleton.NumBones]);
            Bones = _bones.AsReadOnly();
        }

        bool flag2 = false;
        for (int j = 0; j < _bones.Count; j++)
        {
            OVRBone oVRBone = _bones[j] ?? (_bones[j] = new OVRBone());
            oVRBone.Id = (BoneId)_skeleton.Bones[j].Id;
            oVRBone.ParentBoneIndex = _skeleton.Bones[j].ParentBoneIndex;
            Assert.IsTrue(oVRBone.Id >= BoneId.Hand_Start && oVRBone.Id <= BoneId.FullBody_End);
            if (oVRBone.Transform == null)
            {
                flag2 = true;
                oVRBone.Transform = GetBoneTransform(oVRBone.Id);
                if (oVRBone.Transform == null)
                {
                    oVRBone.Transform = new GameObject(BoneLabelFromBoneId(_skeletonType, oVRBone.Id)).transform;
                }
            }

            if (GetBoneTransform(oVRBone.Id) == null)
            {
                oVRBone.Transform.name = BoneLabelFromBoneId(_skeletonType, oVRBone.Id);
            }
        }

        if (flag2)
        {
            for (int k = 0; k < _bones.Count; k++)
            {
                if (!IsValidBone((BoneId)_bones[k].ParentBoneIndex) || IsBodySkeleton(_skeletonType))
                {
                    _bones[k].Transform.SetParent(_bonesGO.transform, worldPositionStays: false);
                }
                else
                {
                    _bones[k].Transform.SetParent(_bones[_bones[k].ParentBoneIndex].Transform, worldPositionStays: false);
                }
            }
        }

        for (int l = 0; l < _bones.Count; l++)
        {
            OVRBone oVRBone3 = _bones[l];
            OVRPlugin.Posef pose = _skeleton.Bones[l].Pose;
            if (_applyBoneTranslations)
            {
                oVRBone3.Transform.localPosition = (flag ? pose.Position.FromFlippedXVector3f() : pose.Position.FromFlippedZVector3f());
            }

            oVRBone3.Transform.localRotation = (flag ? pose.Orientation.FromFlippedXQuatf() : pose.Orientation.FromFlippedZQuatf());
        }
    }

    protected virtual void InitializeBindPose()
    {
        if (!_bindPosesGO)
        {
            _bindPosesGO = new GameObject("BindPoses");
            _bindPosesGO.transform.SetParent(base.transform, worldPositionStays: false);
            _bindPosesGO.transform.localPosition = Vector3.zero;
            _bindPosesGO.transform.localRotation = Quaternion.identity;
        }

        if (_bindPoses != null)
        {
            for (int i = 0; i < _bindPoses.Count; i++)
            {
                _bindPoses[i].Dispose();
            }

            _bindPoses.Clear();
        }

        if (_bindPosesGO != null)
        {
            List<Transform> list = new List<Transform>();
            for (int j = 0; j < _bindPosesGO.transform.childCount; j++)
            {
                list.Add(_bindPosesGO.transform.GetChild(j));
            }

            for (int k = 0; k < list.Count; k++)
            {
                Object.Destroy(list[k].gameObject);
            }
        }

        if (_bindPoses == null || _bindPoses.Count != _bones.Count)
        {
            _bindPoses = new List<OVRBone>(new OVRBone[_bones.Count]);
            BindPoses = _bindPoses.AsReadOnly();
        }

        for (int l = 0; l < _bindPoses.Count; l++)
        {
            OVRBone oVRBone = _bones[l];
            OVRBone oVRBone2 = _bindPoses[l] ?? (_bindPoses[l] = new OVRBone());
            oVRBone2.Id = oVRBone.Id;
            oVRBone2.ParentBoneIndex = oVRBone.ParentBoneIndex;
            Transform obj;
            if (!oVRBone2.Transform)
            {
                Transform transform = (oVRBone2.Transform = new GameObject(BoneLabelFromBoneId(_skeletonType, oVRBone2.Id)).transform);
                obj = transform;
            }
            else
            {
                obj = oVRBone2.Transform;
            }

            obj.localPosition = oVRBone.Transform.localPosition;
            obj.localRotation = oVRBone.Transform.localRotation;
        }

        for (int m = 0; m < _bindPoses.Count; m++)
        {
            if (!IsValidBone((BoneId)_bindPoses[m].ParentBoneIndex) || IsBodySkeleton(_skeletonType))
            {
                _bindPoses[m].Transform.SetParent(_bindPosesGO.transform, worldPositionStays: false);
            }
            else
            {
                _bindPoses[m].Transform.SetParent(_bindPoses[_bindPoses[m].ParentBoneIndex].Transform, worldPositionStays: false);
            }
        }
    }

    private void InitializeCapsules()
    {
        bool flag = _skeletonType.IsOVRHandSkeleton();
        if (!_enablePhysicsCapsules)
        {
            return;
        }

        if (!_capsulesGO)
        {
            _capsulesGO = new GameObject("Capsules");
            _capsulesGO.transform.SetParent(base.transform, worldPositionStays: false);
            _capsulesGO.transform.localPosition = Vector3.zero;
            _capsulesGO.transform.localRotation = Quaternion.identity;
        }

        if (_capsules != null)
        {
            for (int i = 0; i < _capsules.Count; i++)
            {
                _capsules[i].Cleanup();
            }

            _capsules.Clear();
        }

        if (_capsules == null || _capsules.Count != _skeleton.NumBoneCapsules)
        {
            _capsules = new List<OVRBoneCapsule>(new OVRBoneCapsule[_skeleton.NumBoneCapsules]);
            Capsules = _capsules.AsReadOnly();
        }

        for (int j = 0; j < _capsules.Count; j++)
        {
            OVRBone oVRBone = _bones[_skeleton.BoneCapsules[j].BoneIndex];
            OVRBoneCapsule oVRBoneCapsule = _capsules[j] ?? (_capsules[j] = new OVRBoneCapsule());
            oVRBoneCapsule.BoneIndex = _skeleton.BoneCapsules[j].BoneIndex;
            if (oVRBoneCapsule.CapsuleRigidbody == null)
            {
                oVRBoneCapsule.CapsuleRigidbody = new GameObject(BoneLabelFromBoneId(_skeletonType, oVRBone.Id) + "_CapsuleRigidbody").AddComponent<Rigidbody>();
                oVRBoneCapsule.CapsuleRigidbody.mass = 1f;
                oVRBoneCapsule.CapsuleRigidbody.isKinematic = true;
                oVRBoneCapsule.CapsuleRigidbody.useGravity = false;
                oVRBoneCapsule.CapsuleRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }

            GameObject gameObject = oVRBoneCapsule.CapsuleRigidbody.gameObject;
            gameObject.transform.SetParent(_capsulesGO.transform, worldPositionStays: false);
            gameObject.transform.position = oVRBone.Transform.position;
            gameObject.transform.rotation = oVRBone.Transform.rotation;
            if (oVRBoneCapsule.CapsuleCollider == null)
            {
                oVRBoneCapsule.CapsuleCollider = new GameObject(BoneLabelFromBoneId(_skeletonType, oVRBone.Id) + "_CapsuleCollider").AddComponent<CapsuleCollider>();
                oVRBoneCapsule.CapsuleCollider.isTrigger = false;
            }

            Vector3 vector = (flag ? _skeleton.BoneCapsules[j].StartPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[j].StartPoint.FromFlippedZVector3f());
            Vector3 toDirection = (flag ? _skeleton.BoneCapsules[j].EndPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[j].EndPoint.FromFlippedZVector3f()) - vector;
            float magnitude = toDirection.magnitude;
            Quaternion localRotation = Quaternion.FromToRotation(Vector3.right, toDirection);
            oVRBoneCapsule.CapsuleCollider.radius = _skeleton.BoneCapsules[j].Radius;
            oVRBoneCapsule.CapsuleCollider.height = magnitude + _skeleton.BoneCapsules[j].Radius * 2f;
            oVRBoneCapsule.CapsuleCollider.direction = 0;
            oVRBoneCapsule.CapsuleCollider.center = Vector3.right * magnitude * 0.5f;
            GameObject obj = oVRBoneCapsule.CapsuleCollider.gameObject;
            obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
            obj.transform.localPosition = vector;
            obj.transform.localRotation = localRotation;
        }
    }

    protected virtual void Update()
    {
        UpdateSkeleton();
    }

    protected void UpdateSkeleton()
    {
        if (ShouldInitialize())
        {
            Initialize();
        }

        if (!IsInitialized || _dataProvider == null)
        {
            IsDataValid = false;
            IsDataHighConfidence = false;
            return;
        }

        SkeletonPoseData skeletonPoseData = _dataProvider.GetSkeletonPoseData();
        IsDataValid = skeletonPoseData.IsDataValid;
        if (!skeletonPoseData.IsDataValid)
        {
            return;
        }

        if (SkeletonChangedCount != skeletonPoseData.SkeletonChangedCount)
        {
            SkeletonChangedCount = skeletonPoseData.SkeletonChangedCount;
            IsInitialized = false;
            Initialize();
        }

        IsDataHighConfidence = skeletonPoseData.IsDataHighConfidence;
        if (_updateRootPose)
        {
            base.transform.localPosition = skeletonPoseData.RootPose.Position.FromFlippedZVector3f();
            base.transform.localRotation = skeletonPoseData.RootPose.Orientation.FromFlippedZQuatf();
        }

        if (_updateRootScale)
        {
            base.transform.localScale = new Vector3(skeletonPoseData.RootScale, skeletonPoseData.RootScale, skeletonPoseData.RootScale);
        }

        for (int i = 0; i < _bones.Count; i++)
        {
            Transform transform = _bones[i].Transform;
            if (transform == null)
            {
                continue;
            }

            if (IsBodySkeleton(_skeletonType))
            {
                transform.localPosition = skeletonPoseData.BoneTranslations[i].FromFlippedZVector3f();
                transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
            }
            else if (IsHandSkeleton(_skeletonType))
            {
                if (_skeletonType.IsOVRHandSkeleton())
                {
                    transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedXQuatf();
                    if (_bones[i].Id == BoneId.Hand_Start)
                    {
                        transform.localRotation *= wristFixupRotation;
                    }
                }
                else if (_skeletonType.IsOpenXRHandSkeleton())
                {
                    Vector3 vector = skeletonPoseData.BoneTranslations[i].FromFlippedZVector3f();
                    Quaternion quaternion = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
                    int parentBoneIndex = _bones[i].ParentBoneIndex;
                    bool num = IsValidBone((BoneId)parentBoneIndex);
                    Vector3 vector2 = (num ? skeletonPoseData.BoneTranslations[parentBoneIndex] : skeletonPoseData.RootPose.Position).FromFlippedZVector3f();
                    Quaternion rotation = (num ? skeletonPoseData.BoneRotations[parentBoneIndex] : skeletonPoseData.RootPose.Orientation).FromFlippedZQuatf();
                    float num2 = ((skeletonPoseData.RootScale > 0f) ? (1f / skeletonPoseData.RootScale) : 1f);
                    Quaternion quaternion2 = Quaternion.Inverse(rotation);
                    transform.localPosition = quaternion2 * (num2 * (vector - vector2));
                    transform.localRotation = quaternion2 * quaternion;
                }
            }
            else
            {
                transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
            }
        }
    }

    protected void FixedUpdate()
    {
        if (!IsInitialized || _dataProvider == null)
        {
            IsDataValid = false;
            IsDataHighConfidence = false;
            return;
        }

        Update();
        if (!_enablePhysicsCapsules)
        {
            return;
        }

        SkeletonPoseData skeletonPoseData = _dataProvider.GetSkeletonPoseData();
        IsDataValid = skeletonPoseData.IsDataValid;
        IsDataHighConfidence = skeletonPoseData.IsDataHighConfidence;
        for (int i = 0; i < _capsules.Count; i++)
        {
            OVRBoneCapsule oVRBoneCapsule = _capsules[i];
            GameObject gameObject = oVRBoneCapsule.CapsuleRigidbody.gameObject;
            if (skeletonPoseData.IsDataValid && skeletonPoseData.IsDataHighConfidence)
            {
                Transform transform = _bones[oVRBoneCapsule.BoneIndex].Transform;
                if (gameObject.activeSelf)
                {
                    oVRBoneCapsule.CapsuleRigidbody.MovePosition(transform.position);
                    oVRBoneCapsule.CapsuleRigidbody.MoveRotation(transform.rotation);
                }
                else
                {
                    gameObject.SetActive(value: true);
                    oVRBoneCapsule.CapsuleRigidbody.position = transform.position;
                    oVRBoneCapsule.CapsuleRigidbody.rotation = transform.rotation;
                }
            }
            else if (gameObject.activeSelf)
            {
                gameObject.SetActive(value: false);
            }
        }
    }

    public BoneId GetCurrentStartBoneId()
    {
        switch (_skeletonType)
        {
            case SkeletonType.HandLeft:
            case SkeletonType.HandRight:
                return BoneId.Hand_Start;
            case SkeletonType.Body:
                return BoneId.Hand_Start;
            case SkeletonType.FullBody:
                return BoneId.Hand_Start;
            case SkeletonType.XRHandLeft:
            case SkeletonType.XRHandRight:
                return BoneId.Hand_Start;
            default:
                return BoneId.Invalid;
        }
    }

    public BoneId GetCurrentEndBoneId()
    {
        switch (_skeletonType)
        {
            case SkeletonType.HandLeft:
            case SkeletonType.HandRight:
                return BoneId.Hand_End;
            case SkeletonType.XRHandLeft:
            case SkeletonType.XRHandRight:
                return BoneId.XRHand_Max;
            case SkeletonType.Body:
                return BoneId.Body_End;
            case SkeletonType.FullBody:
                return BoneId.FullBody_End;
            default:
                return BoneId.Invalid;
        }
    }

    private BoneId GetCurrentMaxSkinnableBoneId()
    {
        switch (_skeletonType)
        {
            case SkeletonType.HandLeft:
            case SkeletonType.HandRight:
                return BoneId.Hand_MaxSkinnable;
            case SkeletonType.Body:
                return BoneId.Body_End;
            case SkeletonType.FullBody:
                return BoneId.FullBody_End;
            case SkeletonType.XRHandLeft:
            case SkeletonType.XRHandRight:
                return BoneId.XRHand_Max;
            default:
                return BoneId.Invalid;
        }
    }

    public int GetCurrentNumBones()
    {
        SkeletonType skeletonType = _skeletonType;
        if (skeletonType != SkeletonType.None && (uint)skeletonType <= 3u)
        {
            return GetCurrentEndBoneId() - GetCurrentStartBoneId();
        }

        return 0;
    }

    public int GetCurrentNumSkinnableBones()
    {
        SkeletonType skeletonType = _skeletonType;
        if (skeletonType != SkeletonType.None && (uint)skeletonType <= 5u)
        {
            return GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId();
        }

        return 0;
    }

    public static string BoneLabelFromBoneId(SkeletonType skeletonType, BoneId boneId)
    {
        switch (skeletonType)
        {
            case SkeletonType.Body:
                return boneId switch
                {
                    BoneId.Hand_Start => "Body_Root",
                    BoneId.Hand_ForearmStub => "Body_Hips",
                    BoneId.Hand_Thumb0 => "Body_SpineLower",
                    BoneId.Hand_Thumb1 => "Body_SpineMiddle",
                    BoneId.Hand_Thumb2 => "Body_SpineUpper",
                    BoneId.Hand_Thumb3 => "Body_Chest",
                    BoneId.Hand_Index1 => "Body_Neck",
                    BoneId.Hand_Index2 => "Body_Head",
                    BoneId.Hand_Index3 => "Body_LeftShoulder",
                    BoneId.Hand_Middle1 => "Body_LeftScapula",
                    BoneId.Hand_Middle2 => "Body_LeftArmUpper",
                    BoneId.Hand_Middle3 => "Body_LeftArmLower",
                    BoneId.Hand_Ring1 => "Body_LeftHandWristTwist",
                    BoneId.Hand_Ring2 => "Body_RightShoulder",
                    BoneId.Hand_Ring3 => "Body_RightScapula",
                    BoneId.Hand_Pinky0 => "Body_RightArmUpper",
                    BoneId.Hand_Pinky1 => "Body_RightArmLower",
                    BoneId.Hand_Pinky2 => "Body_RightHandWristTwist",
                    BoneId.Hand_Pinky3 => "Body_LeftHandPalm",
                    BoneId.Hand_MaxSkinnable => "Body_LeftHandWrist",
                    BoneId.Hand_IndexTip => "Body_LeftHandThumbMetacarpal",
                    BoneId.Hand_MiddleTip => "Body_LeftHandThumbProximal",
                    BoneId.Hand_RingTip => "Body_LeftHandThumbDistal",
                    BoneId.Hand_PinkyTip => "Body_LeftHandThumbTip",
                    BoneId.Hand_End => "Body_LeftHandIndexMetacarpal",
                    BoneId.XRHand_LittleTip => "Body_LeftHandIndexProximal",
                    BoneId.XRHand_Max => "Body_LeftHandIndexIntermediate",
                    BoneId.Body_LeftHandIndexDistal => "Body_LeftHandIndexDistal",
                    BoneId.Body_LeftHandIndexTip => "Body_LeftHandIndexTip",
                    BoneId.Body_LeftHandMiddleMetacarpal => "Body_LeftHandMiddleMetacarpal",
                    BoneId.Body_LeftHandMiddleProximal => "Body_LeftHandMiddleProximal",
                    BoneId.Body_LeftHandMiddleIntermediate => "Body_LeftHandMiddleIntermediate",
                    BoneId.Body_LeftHandMiddleDistal => "Body_LeftHandMiddleDistal",
                    BoneId.Body_LeftHandMiddleTip => "Body_LeftHandMiddleTip",
                    BoneId.Body_LeftHandRingMetacarpal => "Body_LeftHandRingMetacarpal",
                    BoneId.Body_LeftHandRingProximal => "Body_LeftHandRingProximal",
                    BoneId.Body_LeftHandRingIntermediate => "Body_LeftHandRingIntermediate",
                    BoneId.Body_LeftHandRingDistal => "Body_LeftHandRingDistal",
                    BoneId.Body_LeftHandRingTip => "Body_LeftHandRingTip",
                    BoneId.Body_LeftHandLittleMetacarpal => "Body_LeftHandLittleMetacarpal",
                    BoneId.Body_LeftHandLittleProximal => "Body_LeftHandLittleProximal",
                    BoneId.Body_LeftHandLittleIntermediate => "Body_LeftHandLittleIntermediate",
                    BoneId.Body_LeftHandLittleDistal => "Body_LeftHandLittleDistal",
                    BoneId.Body_LeftHandLittleTip => "Body_LeftHandLittleTip",
                    BoneId.Body_RightHandPalm => "Body_RightHandPalm",
                    BoneId.Body_RightHandWrist => "Body_RightHandWrist",
                    BoneId.Body_RightHandThumbMetacarpal => "Body_RightHandThumbMetacarpal",
                    BoneId.Body_RightHandThumbProximal => "Body_RightHandThumbProximal",
                    BoneId.Body_RightHandThumbDistal => "Body_RightHandThumbDistal",
                    BoneId.Body_RightHandThumbTip => "Body_RightHandThumbTip",
                    BoneId.Body_RightHandIndexMetacarpal => "Body_RightHandIndexMetacarpal",
                    BoneId.Body_RightHandIndexProximal => "Body_RightHandIndexProximal",
                    BoneId.Body_RightHandIndexIntermediate => "Body_RightHandIndexIntermediate",
                    BoneId.Body_RightHandIndexDistal => "Body_RightHandIndexDistal",
                    BoneId.Body_RightHandIndexTip => "Body_RightHandIndexTip",
                    BoneId.Body_RightHandMiddleMetacarpal => "Body_RightHandMiddleMetacarpal",
                    BoneId.Body_RightHandMiddleProximal => "Body_RightHandMiddleProximal",
                    BoneId.Body_RightHandMiddleIntermediate => "Body_RightHandMiddleIntermediate",
                    BoneId.Body_RightHandMiddleDistal => "Body_RightHandMiddleDistal",
                    BoneId.Body_RightHandMiddleTip => "Body_RightHandMiddleTip",
                    BoneId.Body_RightHandRingMetacarpal => "Body_RightHandRingMetacarpal",
                    BoneId.Body_RightHandRingProximal => "Body_RightHandRingProximal",
                    BoneId.Body_RightHandRingIntermediate => "Body_RightHandRingIntermediate",
                    BoneId.Body_RightHandRingDistal => "Body_RightHandRingDistal",
                    BoneId.Body_RightHandRingTip => "Body_RightHandRingTip",
                    BoneId.Body_RightHandLittleMetacarpal => "Body_RightHandLittleMetacarpal",
                    BoneId.Body_RightHandLittleProximal => "Body_RightHandLittleProximal",
                    BoneId.Body_RightHandLittleIntermediate => "Body_RightHandLittleIntermediate",
                    BoneId.Body_RightHandLittleDistal => "Body_RightHandLittleDistal",
                    BoneId.Body_RightHandLittleTip => "Body_RightHandLittleTip",
                    _ => "Body_Unknown",
                };
            case SkeletonType.FullBody:
                return boneId switch
                {
                    BoneId.Hand_Start => "FullBody_Root",
                    BoneId.Hand_ForearmStub => "FullBody_Hips",
                    BoneId.Hand_Thumb0 => "FullBody_SpineLower",
                    BoneId.Hand_Thumb1 => "FullBody_SpineMiddle",
                    BoneId.Hand_Thumb2 => "FullBody_SpineUpper",
                    BoneId.Hand_Thumb3 => "FullBody_Chest",
                    BoneId.Hand_Index1 => "FullBody_Neck",
                    BoneId.Hand_Index2 => "FullBody_Head",
                    BoneId.Hand_Index3 => "FullBody_LeftShoulder",
                    BoneId.Hand_Middle1 => "FullBody_LeftScapula",
                    BoneId.Hand_Middle2 => "FullBody_LeftArmUpper",
                    BoneId.Hand_Middle3 => "FullBody_LeftArmLower",
                    BoneId.Hand_Ring1 => "FullBody_LeftHandWristTwist",
                    BoneId.Hand_Ring2 => "FullBody_RightShoulder",
                    BoneId.Hand_Ring3 => "FullBody_RightScapula",
                    BoneId.Hand_Pinky0 => "FullBody_RightArmUpper",
                    BoneId.Hand_Pinky1 => "FullBody_RightArmLower",
                    BoneId.Hand_Pinky2 => "FullBody_RightHandWristTwist",
                    BoneId.Hand_Pinky3 => "FullBody_LeftHandPalm",
                    BoneId.Hand_MaxSkinnable => "FullBody_LeftHandWrist",
                    BoneId.Hand_IndexTip => "FullBody_LeftHandThumbMetacarpal",
                    BoneId.Hand_MiddleTip => "FullBody_LeftHandThumbProximal",
                    BoneId.Hand_RingTip => "FullBody_LeftHandThumbDistal",
                    BoneId.Hand_PinkyTip => "FullBody_LeftHandThumbTip",
                    BoneId.Hand_End => "FullBody_LeftHandIndexMetacarpal",
                    BoneId.XRHand_LittleTip => "FullBody_LeftHandIndexProximal",
                    BoneId.XRHand_Max => "FullBody_LeftHandIndexIntermediate",
                    BoneId.Body_LeftHandIndexDistal => "FullBody_LeftHandIndexDistal",
                    BoneId.Body_LeftHandIndexTip => "FullBody_LeftHandIndexTip",
                    BoneId.Body_LeftHandMiddleMetacarpal => "FullBody_LeftHandMiddleMetacarpal",
                    BoneId.Body_LeftHandMiddleProximal => "FullBody_LeftHandMiddleProximal",
                    BoneId.Body_LeftHandMiddleIntermediate => "FullBody_LeftHandMiddleIntermediate",
                    BoneId.Body_LeftHandMiddleDistal => "FullBody_LeftHandMiddleDistal",
                    BoneId.Body_LeftHandMiddleTip => "FullBody_LeftHandMiddleTip",
                    BoneId.Body_LeftHandRingMetacarpal => "FullBody_LeftHandRingMetacarpal",
                    BoneId.Body_LeftHandRingProximal => "FullBody_LeftHandRingProximal",
                    BoneId.Body_LeftHandRingIntermediate => "FullBody_LeftHandRingIntermediate",
                    BoneId.Body_LeftHandRingDistal => "FullBody_LeftHandRingDistal",
                    BoneId.Body_LeftHandRingTip => "FullBody_LeftHandRingTip",
                    BoneId.Body_LeftHandLittleMetacarpal => "FullBody_LeftHandLittleMetacarpal",
                    BoneId.Body_LeftHandLittleProximal => "FullBody_LeftHandLittleProximal",
                    BoneId.Body_LeftHandLittleIntermediate => "FullBody_LeftHandLittleIntermediate",
                    BoneId.Body_LeftHandLittleDistal => "FullBody_LeftHandLittleDistal",
                    BoneId.Body_LeftHandLittleTip => "FullBody_LeftHandLittleTip",
                    BoneId.Body_RightHandPalm => "FullBody_RightHandPalm",
                    BoneId.Body_RightHandWrist => "FullBody_RightHandWrist",
                    BoneId.Body_RightHandThumbMetacarpal => "FullBody_RightHandThumbMetacarpal",
                    BoneId.Body_RightHandThumbProximal => "FullBody_RightHandThumbProximal",
                    BoneId.Body_RightHandThumbDistal => "FullBody_RightHandThumbDistal",
                    BoneId.Body_RightHandThumbTip => "FullBody_RightHandThumbTip",
                    BoneId.Body_RightHandIndexMetacarpal => "FullBody_RightHandIndexMetacarpal",
                    BoneId.Body_RightHandIndexProximal => "FullBody_RightHandIndexProximal",
                    BoneId.Body_RightHandIndexIntermediate => "FullBody_RightHandIndexIntermediate",
                    BoneId.Body_RightHandIndexDistal => "FullBody_RightHandIndexDistal",
                    BoneId.Body_RightHandIndexTip => "FullBody_RightHandIndexTip",
                    BoneId.Body_RightHandMiddleMetacarpal => "FullBody_RightHandMiddleMetacarpal",
                    BoneId.Body_RightHandMiddleProximal => "FullBody_RightHandMiddleProximal",
                    BoneId.Body_RightHandMiddleIntermediate => "FullBody_RightHandMiddleIntermediate",
                    BoneId.Body_RightHandMiddleDistal => "FullBody_RightHandMiddleDistal",
                    BoneId.Body_RightHandMiddleTip => "FullBody_RightHandMiddleTip",
                    BoneId.Body_RightHandRingMetacarpal => "FullBody_RightHandRingMetacarpal",
                    BoneId.Body_RightHandRingProximal => "FullBody_RightHandRingProximal",
                    BoneId.Body_RightHandRingIntermediate => "FullBody_RightHandRingIntermediate",
                    BoneId.Body_RightHandRingDistal => "FullBody_RightHandRingDistal",
                    BoneId.Body_RightHandRingTip => "FullBody_RightHandRingTip",
                    BoneId.Body_RightHandLittleMetacarpal => "FullBody_RightHandLittleMetacarpal",
                    BoneId.Body_RightHandLittleProximal => "FullBody_RightHandLittleProximal",
                    BoneId.Body_RightHandLittleIntermediate => "FullBody_RightHandLittleIntermediate",
                    BoneId.Body_RightHandLittleDistal => "FullBody_RightHandLittleDistal",
                    BoneId.Body_RightHandLittleTip => "FullBody_RightHandLittleTip",
                    BoneId.Body_End => "FullBody_LeftUpperLeg",
                    BoneId.FullBody_LeftLowerLeg => "FullBody_LeftLowerLeg",
                    BoneId.FullBody_LeftFootAnkleTwist => "FullBody_LeftFootAnkleTwist",
                    BoneId.FullBody_LeftFootAnkle => "FullBody_LeftFootAnkle",
                    BoneId.FullBody_LeftFootSubtalar => "FullBody_LeftFootSubtalar",
                    BoneId.FullBody_LeftFootTransverse => "FullBody_LeftFootTransverse",
                    BoneId.FullBody_LeftFootBall => "FullBody_LeftFootBall",
                    BoneId.FullBody_RightUpperLeg => "FullBody_RightUpperLeg",
                    BoneId.FullBody_RightLowerLeg => "FullBody_RightLowerLeg",
                    BoneId.FullBody_RightFootAnkleTwist => "FullBody_RightFootAnkleTwist",
                    BoneId.FullBody_RightFootAnkle => "FullBody_RightFootAnkle",
                    BoneId.FullBody_RightFootSubtalar => "FullBody_RightFootSubtalar",
                    BoneId.FullBody_RightFootTransverse => "FullBody_RightFootTransverse",
                    BoneId.FullBody_RightFootBall => "FullBody_RightFootBall",
                    _ => "FullBody_Unknown",
                };
            default:
                if (IsHandSkeleton(skeletonType))
                {
                    if (skeletonType == SkeletonType.HandLeft || skeletonType == SkeletonType.HandRight)
                    {
                        return boneId switch
                        {
                            BoneId.Hand_Start => "Hand_WristRoot",
                            BoneId.Hand_ForearmStub => "Hand_ForearmStub",
                            BoneId.Hand_Thumb0 => "Hand_Thumb0",
                            BoneId.Hand_Thumb1 => "Hand_Thumb1",
                            BoneId.Hand_Thumb2 => "Hand_Thumb2",
                            BoneId.Hand_Thumb3 => "Hand_Thumb3",
                            BoneId.Hand_Index1 => "Hand_Index1",
                            BoneId.Hand_Index2 => "Hand_Index2",
                            BoneId.Hand_Index3 => "Hand_Index3",
                            BoneId.Hand_Middle1 => "Hand_Middle1",
                            BoneId.Hand_Middle2 => "Hand_Middle2",
                            BoneId.Hand_Middle3 => "Hand_Middle3",
                            BoneId.Hand_Ring1 => "Hand_Ring1",
                            BoneId.Hand_Ring2 => "Hand_Ring2",
                            BoneId.Hand_Ring3 => "Hand_Ring3",
                            BoneId.Hand_Pinky0 => "Hand_Pinky0",
                            BoneId.Hand_Pinky1 => "Hand_Pinky1",
                            BoneId.Hand_Pinky2 => "Hand_Pinky2",
                            BoneId.Hand_Pinky3 => "Hand_Pinky3",
                            BoneId.Hand_MaxSkinnable => "Hand_ThumbTip",
                            BoneId.Hand_IndexTip => "Hand_IndexTip",
                            BoneId.Hand_MiddleTip => "Hand_MiddleTip",
                            BoneId.Hand_RingTip => "Hand_RingTip",
                            BoneId.Hand_PinkyTip => "Hand_PinkyTip",
                            _ => "Hand_Unknown",
                        };
                    }

                    return boneId switch
                    {
                        BoneId.Hand_Start => "XRHand_Palm",
                        BoneId.Hand_ForearmStub => "XRHand_Wrist",
                        BoneId.Hand_Thumb0 => "XRHand_ThumbMetacarpal",
                        BoneId.Hand_Thumb1 => "XRHand_ThumbProximal",
                        BoneId.Hand_Thumb2 => "XRHand_ThumbDistal",
                        BoneId.Hand_Thumb3 => "XRHand_ThumbTip",
                        BoneId.Hand_Index1 => "XRHand_IndexMetacarpal",
                        BoneId.Hand_Index2 => "XRHand_IndexProximal",
                        BoneId.Hand_Index3 => "XRHand_IndexIntermediate",
                        BoneId.Hand_Middle1 => "XRHand_IndexDistal",
                        BoneId.Hand_Middle2 => "XRHand_IndexTip",
                        BoneId.Hand_Middle3 => "XRHand_MiddleMetacarpal",
                        BoneId.Hand_Ring1 => "XRHand_MiddleProximal",
                        BoneId.Hand_Ring2 => "XRHand_MiddleIntermediate",
                        BoneId.Hand_Ring3 => "XRHand_MiddleDistal",
                        BoneId.Hand_Pinky0 => "XRHand_MiddleTip",
                        BoneId.Hand_Pinky1 => "XRHand_RingMetacarpal",
                        BoneId.Hand_Pinky2 => "XRHand_RingProximal",
                        BoneId.Hand_Pinky3 => "XRHand_RingIntermediate",
                        BoneId.Hand_MaxSkinnable => "XRHand_RingDistal",
                        BoneId.Hand_IndexTip => "XRHand_RingTip",
                        BoneId.Hand_MiddleTip => "XRHand_LittleMetacarpal",
                        BoneId.Hand_RingTip => "XRHand_LittleProximal",
                        BoneId.Hand_PinkyTip => "XRHand_LittleIntermediate",
                        BoneId.Hand_End => "XRHand_LittleDistal",
                        BoneId.XRHand_LittleTip => "XRHand_LittleTip",
                        _ => "XRHand_Unknown",
                    };
                }

                return "Skeleton_Unknown";
        }
    }

    internal static bool IsBodySkeleton(SkeletonType type)
    {
        if (type != SkeletonType.Body)
        {
            return type == SkeletonType.FullBody;
        }

        return true;
    }

    private static bool IsHandSkeleton(SkeletonType type)
    {
        return type.IsHand();
    }
}
#if false // Decompilation log
'363' items in cache
------------------
Resolve: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\NetStandard\ref\2.1.0\netstandard.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll'
------------------
Resolve: 'Unity.RenderPipelines.Universal.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.RenderPipelines.Universal.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.RenderPipelines.Universal.Runtime.dll'
------------------
Resolve: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.AnimationModule.dll'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\UnityEngine.UI.dll'
------------------
Resolve: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.XRModule.dll'
------------------
Resolve: 'UnityEngine.VRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.VRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.VRModule.dll'
------------------
Resolve: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.AudioModule.dll'
------------------
Resolve: 'Unity.InputSystem, Version=1.14.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.InputSystem, Version=1.14.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.InputSystem.dll'
------------------
Resolve: 'Unity.XR.OpenXR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.XR.OpenXR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.XR.OpenXR.dll'
------------------
Resolve: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEditor.CoreModule.dll'
------------------
Resolve: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.TextMeshPro.dll'
------------------
Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll'
------------------
Resolve: 'UnityEngine.AndroidJNIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AndroidJNIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.AndroidJNIModule.dll'
------------------
Resolve: 'UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.AssetBundleModule.dll'
------------------
Resolve: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.UIModule.dll'
------------------
Resolve: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.IMGUIModule.dll'
------------------
Resolve: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.Physics2DModule.dll'
------------------
Resolve: 'Unity.XR.OpenXR.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.XR.OpenXR.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.XR.OpenXR.Editor.dll'
------------------
Resolve: 'UnityEngine.SubsystemsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.SubsystemsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.SubsystemsModule.dll'
------------------
Resolve: 'UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.ImageConversionModule.dll'
------------------
Resolve: 'Unity.XR.Management, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.XR.Management, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Unity.XR.Management.dll'
------------------
Resolve: 'Meta.XR.Editor.Callbacks, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Meta.XR.Editor.Callbacks, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Users\G14\GitHub\XRTeleopClient\Library\ScriptAssemblies\Meta.XR.Editor.Callbacks.dll'
------------------
Resolve: 'UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.TextRenderingModule.dll'
------------------
Resolve: 'UnityEngine.JSONSerializeModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.JSONSerializeModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.JSONSerializeModule.dll'
------------------
Resolve: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\Managed\UnityEngine\UnityEngine.InputLegacyModule.dll'
------------------
Resolve: 'System.Runtime.InteropServices, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.InteropServices, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.1.0.0', Got: '4.1.2.0'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Data\NetStandard\compat\2.1.0\shims\netstandard\System.Runtime.InteropServices.dll'
------------------
Resolve: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
#endif
