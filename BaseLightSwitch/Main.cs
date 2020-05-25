namespace BaseLightSwitch
{
    using Harmony;
    using System.Reflection;

    public class Main
    {
        public static void Patch()
        {
            // Patch BaseLightSwitch prefab in the game
            var baseLightSwitchPrefab = new BaseLightSwitchPrefab();
            baseLightSwitchPrefab.Patch();

            // Patch OnProtoSerialize and OnProtoDeserialize methods (used to save and restore bases/cyclops lights state)
            var harmony = HarmonyInstance.Create("baselightswitch");
            harmony.Patch(typeof(SubRoot).GetMethod(nameof(SubRoot.OnProtoSerialize), BindingFlags.Public | BindingFlags.Instance), null, new HarmonyMethod(typeof(SubRootFixer).GetMethod(nameof(SubRootFixer.OnProtoSerialize_Postfix), BindingFlags.Public | BindingFlags.Static)));
            harmony.Patch(typeof(SubRoot).GetMethod(nameof(SubRoot.OnProtoDeserialize), BindingFlags.Public | BindingFlags.Instance), null, new HarmonyMethod(typeof(SubRootFixer).GetMethod(nameof(SubRootFixer.OnProtoDeserialize_Postfix), BindingFlags.Public | BindingFlags.Static)));
        }

    }
}
