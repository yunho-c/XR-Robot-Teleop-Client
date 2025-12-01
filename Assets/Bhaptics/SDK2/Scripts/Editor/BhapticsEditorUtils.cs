using System.Collections.Generic;

namespace Bhaptics.SDK2.Scripts.Editor
{
    public class BhapticsEditorUtils
    {
        public static List<MappingMetaData> EditorGetEventList(string appId, string apiKey, int lastVersion, out int status)
        {
            var res = bhaptics_editor.EditorGetEventList(appId, apiKey, lastVersion, out int code);
            status = code;
            return res;
        }

        public static string EditorGetSettings(string appId, string apiKey, int lastVersion, out int status)
        {
            var str = bhaptics_editor.EditorGetSettings(appId, apiKey, lastVersion, out int code);
            BhapticsLogManager.LogFormat("EditorGetSettings {0} {1}", code, str);
            status = code;
            return str;
        }
        
        public static bool EditorReInitialize(string appId, string apiKey, string json)
        {
            BhapticsLogManager.LogFormat("[bHaptics] BhapticsLibrary - ReInitialize() {0} {1}", apiKey, appId);
            return bhaptics_library.reInitMessage(apiKey, appId, json);
        }

    }
}