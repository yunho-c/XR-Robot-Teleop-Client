using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Bhaptics.SDK2
{
    public class BhapticsSDK2 : MonoBehaviour
    {
        private static BhapticsSDK2 instance;
        public static bool IsInitialized { get; private set; }

        [Header("Only for PC")]
        [Tooltip("If bHaptics Player(PC) is not turned on when this program starts, it automatically runs bHaptics Player.")]
        [SerializeField] private bool autoRunBhapticsPlayer = false;
        private bool autoRequestBluetoothPermission = true;

        private BhapticsSettings bhapticsSettings;



        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);

            bhapticsSettings = BhapticsSettings.Instance;

            if (string.IsNullOrEmpty(bhapticsSettings.AppId))
            {
                Debug.LogError("[bHaptics] Please set API_ID.");
                return;
            }

            BhapticsLogManager.Log($"[bHaptics] {bhapticsSettings.AppId} {bhapticsSettings.ApiKey}");
            BhapticsLibrary.Initialize(bhapticsSettings.AppId, bhapticsSettings.ApiKey, bhapticsSettings.DefaultDeploy, autoRequestBluetoothPermission);

            IsInitialized = BhapticsLibrary.IsBhapticsAvailable(autoRunBhapticsPlayer);
            BhapticsLogManager.Log($"[bHaptics] player IsBhapticsAvailable {IsInitialized}");
            BhapticsLogManager.Log("[bHaptics] Initialized.");
            
            var hapticDevices = BhapticsLibrary.GetDevices();
            BhapticsLogManager.Log($"[bHaptics] devices {hapticDevices.Count}");
        }

        private void OnApplicationFocus(bool pauseStatus)
        {
            if (pauseStatus)
            {
                BhapticsLibrary.OnApplicationFocus();
            }
            else
            {
                BhapticsLibrary.OnApplicationPause();
            }
        }

        private void OnDestroy()
        {
            IsInitialized = false;

            if (instance == null || instance.GetInstanceID() != this.GetInstanceID())
            {
                return;
            }

            BhapticsLibrary.Destroy();
            
        }

        private void OnApplicationQuit()
        {
            BhapticsLibrary.OnApplicationQuit();
        }

#if UNITY_EDITOR
        public static string GetBhapticsFolderParentPath(bool localToAssetsFolder = false)
        {
            BhapticsSettings asset = ScriptableObject.CreateInstance<BhapticsSettings>();
            UnityEditor.MonoScript scriptAsset = UnityEditor.MonoScript.FromScriptableObject(asset);

            string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(scriptAsset);

            FileInfo settingsScriptFileInfo = new FileInfo(scriptPath);

            string fullPath = settingsScriptFileInfo.Directory.Parent.Parent.FullName;

            if (localToAssetsFolder == false)
            {
                return fullPath;
            }

            DirectoryInfo assetsDirectoryInfo = new DirectoryInfo(Application.dataPath);
            string localPath = fullPath.Substring(assetsDirectoryInfo.Parent.FullName.Length + 1);
            return localPath;
        }

        public static string GetResourcePath(bool localToAssetsFolder = false)
        {
            string path = Path.Combine(GetBhapticsFolderParentPath(localToAssetsFolder), "Resources");

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);

            }

            return path;
        }

        public static string GetScriptPath(bool localToAssetsFolder = false)
        {
            string path = Path.Combine(GetBhapticsFolderParentPath(localToAssetsFolder), "Scripts");

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);

            }

            return path;
        }
#endif
    }
}

