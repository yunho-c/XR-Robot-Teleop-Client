using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Bhaptics.SDK2
{
    [Serializable]
    internal class MappingMessage
    {
        public bool status;
        public List<MappingMetaData> message;

        internal static MappingMessage CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MappingMessage>(jsonString);
        }
    }
    
    public class bhaptics_editor
    {
        private const string ModuleName = "bhaptics_editor";

        private static bool _isInitialized = false;
        private static readonly object _initLock = new object();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool initialize_runtime();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void free_string(IntPtr ptr);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_haptic_messages(string app_id, string api_key, int version, out int status);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_haptic_mappings(string app_id, string api_key, int version, out int status);

        private static bool _initializationSucceeded;

        static bhaptics_editor()
        {
            try
            {
                _initializationSucceeded = initialize_runtime();
                if (!_initializationSucceeded)
                {
                    Console.WriteLine("Failed to initialize bHaptics runtime");
                }
            }
            catch (Exception ex)
            {
                _initializationSucceeded = false;
                Console.WriteLine($"Error initializing bHaptics runtime: {ex.Message}");
            }
        }

        private static void EnsureInitialized()
        {
            if (!_initializationSucceeded)
            {
                throw new InvalidOperationException("bHaptics runtime failed to initialize");
            }
        }

        public static string GetHapticMessages(string app_id, string api_key, int version, out int status)
        {
            EnsureInitialized();

            IntPtr resultPtr = get_haptic_messages(app_id, api_key, version, out status);
            if (resultPtr == IntPtr.Zero)
            {
                status = 999;
                return null;
            }

            try
            {
                return Marshal.PtrToStringAnsi(resultPtr);
            }
            finally
            {
                free_string(resultPtr);
            }
        }

        public static string GetHapticMappings(string app_id, string api_key, int version, out int status)
        {
            EnsureInitialized();

            IntPtr resultPtr = get_haptic_mappings(app_id, api_key, version, out status);
            if (resultPtr == IntPtr.Zero)
            {
                status = 999;
                return null;
            }

            try
            {
                return Marshal.PtrToStringAnsi(resultPtr);
            }
            finally
            {
                free_string(resultPtr);
            }
        }

        public static List<MappingMetaData> EditorGetEventList(string appId, string apiKey, int lastVersion, out int status)
        {
            var str = GetHapticMappings(appId, apiKey, lastVersion, out int code);

            status = code;
            if (code == 0)
            {
                var mappingMessage = MappingMessage.CreateFromJSON(str);
                return mappingMessage.message;
            }
            BhapticsLogManager.Log($"EditorGetEventList: {code}");
            return new List<MappingMetaData>();
        }

        public static string EditorGetSettings(string appId, string apiKey, int lastVersion, out int status2)
        {
            var str = GetHapticMessages(appId, apiKey, lastVersion, out int status);
            
            status2 = status;
            if (status == 0)
            {
                return str;
            }
            
            return "";
        }
    }
    
}
