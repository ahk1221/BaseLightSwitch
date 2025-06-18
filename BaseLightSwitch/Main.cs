namespace BaseLightSwitch
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using BepInEx;
    using BepInEx.Logging;
#if NAUTILUS
    using HarmonyLib;
#else // SML Helper (Living Large update)
    using Harmony;
#endif

    [BepInPlugin(GUID, NAME, VERSION)]
#if NAUTILUS
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
#else // SML Helper (Living Large update)
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
#endif
    [DisallowMultipleComponent]
    public class BaseLightSwitch_EntryPoint : BaseUnityPlugin
    {
        public const string GUID = "com.ahk1221.baselightswitch";
        public const string NAME = "BaseLightSwitch";
        public const string VERSION = "2.1.0";

        private static bool _initialized = false;
        private static bool _success = true;
        public static ManualLogSource _logger = null;

        public void Awake()
        {
            BaseLightSwitch_EntryPoint._logger = Logger;
            if (!BaseLightSwitch_EntryPoint._initialized)
            {
                BaseLightSwitch_EntryPoint._initialized = true;
                Logger.LogInfo($"Initializing {NAME} mod...");
                try
                {
                    // Register BaseLightSwitch in the game
                    BaseLightSwitchPrefab baseLightSwitchPrefab = new BaseLightSwitchPrefab();
                    if (baseLightSwitchPrefab != null)
                    {
#if NAUTILUS
                        baseLightSwitchPrefab.Register();
#else // SML Helper (Living Large update)
                        baseLightSwitchPrefab.Patch();
#endif
                    }
                    else
                    {
                        BaseLightSwitch_EntryPoint._success = false;
                        Logger.LogError($"Failed to register {NAME} item in the game.");
                    }

                    // Patch OnProtoSerialize and OnProtoDeserialize methods. Used to save and restore lights state (per base/cyclops).
#if NAUTILUS
                    var harmony = new Harmony(GUID);
#else // SML Helper (Living Large update)
                    var harmony = HarmonyInstance.Create(GUID);
#endif
                    if (harmony != null)
                    {
                        harmony.Patch(typeof(SubRoot).GetMethod(nameof(SubRoot.OnProtoSerialize), BindingFlags.Public | BindingFlags.Instance), null, new HarmonyMethod(typeof(SubRootFixer).GetMethod(nameof(SubRootFixer.OnProtoSerialize_Postfix), BindingFlags.Public | BindingFlags.Static)));
                        harmony.Patch(typeof(SubRoot).GetMethod(nameof(SubRoot.OnProtoDeserialize), BindingFlags.Public | BindingFlags.Instance), null, new HarmonyMethod(typeof(SubRootFixer).GetMethod(nameof(SubRootFixer.OnProtoDeserialize_Postfix), BindingFlags.Public | BindingFlags.Static)));
                    }
                    else
                    {
                        BaseLightSwitch_EntryPoint._success = false;
                        Logger.LogError("Failed to patch SubRoot with Harmony.");
                    }
                }
                catch (Exception ex)
                {
                    BaseLightSwitch_EntryPoint._success = false;
                    Logger.LogError(string.Format("Exception caught! Message=[{0}] StackTrace=[{1}]", ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                        Logger.LogError(string.Format("Inner exception => Message=[{0}] StackTrace=[{1}]", ex.InnerException.Message, ex.InnerException.StackTrace));
                }
                if (BaseLightSwitch_EntryPoint._success)
                    Logger.LogInfo($"{NAME} mod initialized successfully.");
                else
                    Logger.LogError($"{NAME} mod initialization failed.");
            }
        }
    }
}
