using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bhaptics.SDK2.Universal
{

    public static class MessageType
    {
        public const string ServerTokenMessage = "ServerTokenMessage";
        public const string ServerDeviceListMessage = "ServerDeviceListMessage";

        public const string SdkPlay = "SdkPlay";
        public const string SdkPlayWithStartTime = "SdkPlayWithStartTime";
        public const string SdkPing = "SdkPing";
        public const string SdkPingAll = "SdkPingAll";
        public const string SdkResume = "SdkResume";
        public const string SdkPause = "SdkPause";
        public const string SdkPingToServer = "SdkPingToServer";
        public const string SdkSwapPosition = "SdkSwapPosition";
        public const string SdkPlayDotMode = "SdkPlayDotMode";
        public const string SdkStopByRequestId = "SdkStopByRequestId";
        public const string SdkStopByEventId = "SdkStopByEventId";
        public const string SdkStopAll = "SdkStopAll";
        public const string SdkRequestAuth = "SdkRequestAuth";
        public const string SdkRequestAuthInit = "SdkRequestAuthInit";
        public const string SdkRequestReInit = "SdkRequestReInit";
        public const string SdkPlayLoop = "SdkPlayLoop";
        public const string SdkPlayWaveformMode = "SdkPlayWaveformMode";
        public const string SdkPlayPathMode = "SdkPlayPathMode";

    }

    [Serializable]
    public class PlayMessage
    {
        public string eventName;
        public int requestId;
        public int startMillis = 0;
        public float intensity = 1f;
        public float duration = 1f;
        public float offsetAngleX = 0f;
        public float offsetY = 0f;
    }
    
    [Serializable]
    public class PauseMessage
    {
        public string eventName;
    }
    [Serializable]
    public class ResumeMessage
    {
        public string eventName;
    }

    [Serializable]
    public class PlayLoopMessage
    {
        public string eventName;
        public int requestId;
        public float intensity = 1f;
        public float duration = 1f;
        public float offsetAngleX = 0f;
        public float offsetY = 0f;
        public int interval = 0;
        public int maxCount = 1000000;
    }

    [Serializable]
    public class PlayDotModeMessage
    {
        public int requestId = 0;
        public int pos = 0;
        public int durationMillis = 0;
        public int[] motors;
    }

    [Serializable]
    public class PlayWaveformModeMessage
    {
        public int requestId = 0;
        public int pos = 0;
        public int[] motorValues;
        public int[] playTimeValues;
        public int[] shapeValues;
    }

    [Serializable]
    public class PlayPathModeMessage
    {
        public int requestId = 0;
        public int pos = 0;
        public int durationMillis = 0;
        public float[] x;
        public float[] y;
        public int[] intensity;
    }

    [Serializable]
    public class AuthenticationMessage
    {
        public string sdkApiKey;
        public string applicationId;
        public int version;
    }

    [Serializable]
    public class TokenMessage
    {
        public string token;
        public string tokenKey;
    }

    [Serializable]
    public class TactHubMessage
    {
        public string type;
        public string message;

        public static TactHubMessage SdkPlay(PlayMessage message)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(message),
                type =  MessageType.SdkPlay
            };
        }

        public static TactHubMessage SdkPlayLoop(PlayLoopMessage message)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(message),
                type = MessageType.SdkPlayLoop
            };
        }

        public static TactHubMessage SdkPlayDotMode(PlayDotModeMessage message)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(message),
                type = MessageType.SdkPlayDotMode
            };
        }

        public static TactHubMessage SdkPlayWaveformMode(PlayWaveformModeMessage message)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(message),
                type = MessageType.SdkPlayWaveformMode
            };
        }

        public static TactHubMessage SdkPlayPathMode(PlayPathModeMessage message)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(message),
                type = MessageType.SdkPlayPathMode
            };
        }

        public static TactHubMessage InitializeMessage(string appId, string apiKey)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(new AuthenticationMessage()
                {
                    applicationId = appId,
                    sdkApiKey = apiKey,
                    version = 2,
                }),
                type =  MessageType.SdkRequestAuthInit
            };
        }

        public static TactHubMessage SdkStopByRequestId(int requestId)
        {
            return new TactHubMessage()
            {
                message = requestId.ToString(),
                type = MessageType.SdkStopByRequestId,
            };
        }
        public static TactHubMessage SdkStopByEventId(string eventId)
        {
            return new TactHubMessage()
            {
                message = eventId,
                type = MessageType.SdkStopByEventId,
            };
        }

        public static TactHubMessage SdkPingServer()
        {
            return new TactHubMessage()
            {
                message = "",
                type = MessageType.SdkPingToServer,
            };
        }
        public static TactHubMessage SdkPing(string address)
        {
            return new TactHubMessage()
            {
                message = address,
                type = MessageType.SdkPing,
            };
        }
        public static TactHubMessage SdkSwapPosition(string address)
        {
            return new TactHubMessage()
            {
                message = address,
                type = MessageType.SdkSwapPosition,
            };
        }
        public static TactHubMessage SdkPingAll()
        {
            return new TactHubMessage()
            {
                type = MessageType.SdkPingAll,
            };
        }
        public static TactHubMessage SdkResume(string eventName)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(new ResumeMessage()
                {
                    eventName = eventName,
                }),
                type = MessageType.SdkResume,
            };
        }
        public static TactHubMessage SdkPause(string eventName)
        {
            return new TactHubMessage()
            {
                message = JsonUtility.ToJson(new PauseMessage()
                {
                    eventName = eventName,
                }),
                type = MessageType.SdkPause,
            };
        }
    

        public static readonly TactHubMessage StopAll = new TactHubMessage()
        {
            type = MessageType.SdkStopAll,
        };

    }

}
