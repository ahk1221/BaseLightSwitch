namespace BaseLightSwitch
{
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class SubRootFixer
    {
        public static void OnProtoSerialize_Postfix(SubRoot __instance, ProtobufSerializer serializer)
        {
            // Get base/cyclops prefab ID
            var prefabId = __instance.GetComponent<PrefabIdentifier>();
            if (prefabId == null) return; // Return if we were not able to get or set unique prefab ID

            // Prepare save path
            string saveFolderPath = Path.Combine(Path.Combine(@"./SNAppData/SavedGames/", SaveLoadManager.main.GetCurrentSlot()), "BaseLightSwitch");
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);

            // Get base/cyclops lights state
            FieldInfo isLightsOnField = typeof(SubRoot).GetField("subLightsOn", BindingFlags.Instance | BindingFlags.NonPublic);
            bool isLightsOn = (bool)isLightsOnField.GetValue(__instance);

            // Save base/cyclops lights state
            File.WriteAllText(Path.Combine(saveFolderPath, "lightsstate_" + prefabId.Id + ".txt"), isLightsOn ? "1" : "0", Encoding.UTF8);
        }

        public static void OnProtoDeserialize_Postfix(SubRoot __instance, ProtobufSerializer serializer)
        {
            // Get base/cyclops prefab ID
            var prefabId = __instance.GetComponent<PrefabIdentifier>();
            if (prefabId == null) return; // Return if we were not able to get unique prefab ID

            // Get save folder path
            string saveFolderPath = Path.Combine(Path.Combine(@"./SNAppData/SavedGames/", SaveLoadManager.main.GetCurrentSlot()), "BaseLightSwitch");
            if (!Directory.Exists(saveFolderPath)) return; // Return if save folder does not exist

            // Get save file path
            string saveFilePath = Path.Combine(saveFolderPath, "lightsstate_" + prefabId.Id + ".txt");
            if (!File.Exists(saveFilePath)) return; // Return if there's no data saved for this base/cyclops

            // Get base/cyclops saved lights state
            string savedData = File.ReadAllText(saveFilePath, Encoding.UTF8);
            bool savedLightsState = (string.IsNullOrEmpty(savedData) || savedData != "0");

            // Attach a small component that will restore base/cyclops lights state
            __instance.gameObject.AddComponent<SubRootRestoreLightsState>().IsLightsOn = savedLightsState;
        }
    }
}
