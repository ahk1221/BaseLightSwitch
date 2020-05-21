using System;
using System.Collections.Generic;
using UnityEngine;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System.Reflection;
using System.IO;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Handlers;

namespace BaseLightSwitch
{
    public class BaseLightSwitchPrefab : Buildable
    {
        public BaseLightSwitchPrefab() : base("LightSwitch", "Light Switch", "A light switch that toggles the base's lighting state.")
        {
            // Load the asset bundle before doing anything.
            OnStartedPatching += LoadAssetBundle;

            // Register custom lines
            OnFinishedPatching += RegisterCustomLines;
        }

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public override string AssetsFolder => "BaseLightSwitch/Assets";
        public override string IconFileName => "LightIcon.png";

        private AssetBundle modAB;
        private GameObject loadedPrefab;

        private void RegisterCustomLines()
        {
            LanguageHandler.SetLanguageLine("ToggleLightsBase", "Toggle Lights");
        }

        private void LoadAssetBundle()
        {
            // Stack Overflow: https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path)), "lightswitch.assets");

            modAB = AssetBundle.LoadFromFile(path);

            if (modAB == null) throw new Exception("Light Switch AssetBundle not found! Path: " + path);

            // Load GameObject
            var lightSwitch = modAB.LoadAsset<GameObject>("LightSwitch");
            PrefabUtils.AddBasicComponents(ref lightSwitch, "LightSwitch");

            var constructable = lightSwitch.AddComponent<Constructable>();
            constructable.allowedOnWall = true;
            constructable.allowedOnGround = false;
            constructable.allowedInSub = true;
            constructable.allowedInBase = true;
            constructable.allowedOnCeiling = false;
            constructable.allowedOutside = false;
            constructable.techType = TechType;
            constructable.model = lightSwitch.FindChild("model");

            var bounds = lightSwitch.AddComponent<ConstructableBounds>();

            var techTag = lightSwitch.AddComponent<TechTag>();
            techTag.type = TechType;

            var collider = lightSwitch.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.43f, 0.25f, 0.07f);

            var rb = lightSwitch.GetComponent<Rigidbody>();
            MonoBehaviour.DestroyImmediate(rb);

            var lightToggle = lightSwitch.AddComponent<BaseLightToggle>();

            var prefabId = lightSwitch.GetComponent<PrefabIdentifier>();
            if (prefabId == null) prefabId = lightSwitch.AddComponent<PrefabIdentifier>();
            prefabId.ClassId = "LightSwitch";

            loadedPrefab = lightSwitch;
        }

        public override UnityEngine.GameObject GetGameObject()
        {
            return loadedPrefab;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Titanium, 2)
                },
                craftAmount = 1
            };
        }
    }
}
