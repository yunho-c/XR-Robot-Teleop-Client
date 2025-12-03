using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Bhaptics.SDK2
{
    public class bhaptics_library
    {
        private const string ModuleName = "bhaptics_library";
        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool registryAndInit(string sdkAPIKey, string workspaceId, string initData);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool registryAndInitHost(string sdkAPIKey, string workspaceId, string initData, string url);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool wsIsConnected();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wsClose();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool reInitMessage(string sdkAPIKey, string workspaceId, string initData);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int play(string key);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool stop(int key);
        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool stopByEventId(string eventId);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool stopAll();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isPlaying();
        
        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isPlayingByRequestId(int key);
        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isPlayingByEventId(string eventId);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isbHapticsConnected(int position);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ping(string address);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool pingAll();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool swapPosition(string address);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getDeviceInfoJson();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isPlayerInstalled();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool isPlayerRunning();

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool launchPlayer(bool b);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bHapticsGetHapticMessage(string apiKey, string appId, int lastVersion,
            out int status);
        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bHapticsGetHapticMappings(string apiKey, string appId, int lastVersion,
            out int status);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int playDot(int requestId, int position, int durationMillis, int[] motors, int size);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int playWaveform(int requestId, int position, int[] motorValues, int[] playTimeValues, int[] shapeValues, int releatCount, int motorLen);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int playPath(int requestId, int position, int durationMillis, float[] xValues, float[] yValues, int[] intensityValues, int Len);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int playLoop(string key, int requestId, float intensity, float duration, float angleX, float offsetY, int interval, int maxCount);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pause(string key);

        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int resume(string key);



        [DllImport(ModuleName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int playWithStartTime(string key, int requestId, int startMillis, float intensity, float duration, float angleX, float offsetY);

        // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
        public static List<HapticDevice> GetDevices()
        {
            IntPtr ptr = getDeviceInfoJson();

            var devicesStr = PtrToStringUtf8(ptr);

            if (devicesStr.Length == 0)
            {
                BhapticsLogManager.LogFormat("GetDevices() empty. {0}", devicesStr);
                return new List<HapticDevice>();
            }
            var hapticDevices = JsonUtility.FromJson<DeviceListMessage>("{\"devices\":" + devicesStr + "}");

            return BhapticsHelpers.Convert(hapticDevices.devices);
        }

        private static string PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return "";
            }

            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
            {
                return "";
            }

            byte[] array = new byte[len];
            Marshal.Copy(ptr, array, 0, len);
            return System.Text.Encoding.UTF8.GetString(array);
        }
    }
}