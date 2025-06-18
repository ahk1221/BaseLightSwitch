namespace BaseLightSwitch
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using UnityEngine;
#if NAUTILUS
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Assets;
    using Nautilus.Assets.Gadgets;
    using Nautilus.Crafting;
    using Nautilus.Utility;
    using Nautilus.Handlers;
    using static CraftData;
#else // SML Helper (Living Large update)
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;
    using SMLHelper.V2.Handlers;
#endif

#if NAUTILUS
    public class BaseLightSwitchPrefab : CustomPrefab
    {
        [SetsRequiredMembers]
        public BaseLightSwitchPrefab() : base("LightSwitch", "Light Switch", "A light switch that toggles the base's lighting state.", ImageUtils.LoadSpriteFromFile("./BepInEx/plugins/BaseLightSwitch/Assets/LightIcon.png"))
        {
            // Load the asset
            LoadAssetBundle();
            // Register language line and sprite image
            RegisterCustomLineAndSprite();
            // Define the recipe
            CraftDataHandler.SetRecipeData(this.Info.TechType, new RecipeData()
            {
                Ingredients = new List<Ingredient>() { new Ingredient(TechType.Titanium, 2) },
                craftAmount = 1
            });
            // Set as buildable
            CraftDataHandler.AddBuildable(this.Info.TechType);
            // Add to PDA group category
            CraftDataHandler.AddToGroup(TechGroup.Miscellaneous, TechCategory.Misc, this.Info.TechType);
            // Unlock at start
            KnownTechHandler.UnlockOnStart(this.Info.TechType);
            // Set the gameobject prefab
            this.SetGameObject(this.loadedPrefab);
        }
#else // SML Helper (Living Large update)
    public class BaseLightSwitchPrefab : Buildable
    {
        public BaseLightSwitchPrefab() : base("LightSwitch", "Light Switch", "A light switch that toggles the base's lighting state.")
        {
            // Load the asset bundle before doing anything.
            OnStartedPatching += LoadAssetBundle;

            // Register language text and item sprite
            OnFinishedPatching += RegisterCustomLineAndSprite;
        }

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public override string AssetsFolder => "BaseLightSwitch/Assets";
        public override string IconFileName => "LightIcon.png";
#endif

        private AssetBundle modAB;
        private GameObject loadedPrefab;

        private string AssetsFolderPath() => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets").Replace('\\', '/');

        private void RegisterCustomLineAndSprite()
        {
            LanguageHandler.SetLanguageLine("ToggleLightsBase", "Toggle Lights");
#if !NAUTILUS // SML Helper (Living Large update)
            string path = Path.Combine(this.AssetsFolderPath(), "LightIcon.png").Replace('\\', '/');
            if (File.Exists(path))
                SpriteHandler.RegisterSprite(this.TechType, path);
            else
                Debug.LogError("Light Switch sprite icon not found! Path: " + path);
#endif
        }

        private void LoadAssetBundle()
        {
            // Load the AssetBundle
            string path = Path.Combine(this.AssetsFolderPath(), "lightswitch.assets").Replace('\\', '/');
            modAB = AssetBundle.LoadFromFile(path);
            if (modAB == null) throw new Exception("Light Switch AssetBundle not found! Path: " + path);

            // Load GameObject
            var lightSwitch = modAB.LoadAsset<GameObject>("LightSwitch");

            // Grab model
            var lightSwitchModel = lightSwitch.FindChild("model").FindChild("LIGHTSWITCH");

            // Remove RigidBody
            var rb = lightSwitch.GetComponent<Rigidbody>();
            if (rb != null) MonoBehaviour.DestroyImmediate(rb);

            // Ensure TechTag validity
            var techTag = lightSwitch.EnsureComponent<TechTag>();
#if NAUTILUS
            techTag.type = this.Info.TechType;
#else // SML Helper (Living Large update)
            techTag.type = this.TechType;
#endif

            // Ensure PrefabIdentifier validity
            var prefabId = lightSwitch.EnsureComponent<PrefabIdentifier>();
            prefabId.ClassId = "LightSwitch";

            // Ensure LargeWorldEntity validity
            var lwe = lightSwitch.EnsureComponent<LargeWorldEntity>();
            lwe.cellLevel = LargeWorldEntity.CellLevel.Near;

            // Apply Subnautica shaders
            MaterialUtils.ApplySNShaders(lightSwitch);

            // Add Constructable
            var constructable = lightSwitch.AddComponent<Constructable>();
            constructable.allowedOnWall = true;
            constructable.allowedOnGround = false;
            constructable.allowedInSub = true;
            constructable.allowedInBase = true;
            constructable.allowedOnCeiling = false;
            constructable.allowedOutside = false;
            constructable.model = lightSwitchModel;
#if NAUTILUS
            constructable.techType = this.Info.TechType;
#else // SML Helper (Living Large update)
            constructable.techType = this.TechType;
#endif

            // Add ConstructableBounds
            var bounds = lightSwitch.AddComponent<ConstructableBounds>();

            // Add BoxCollider
            var collider = lightSwitch.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.43f, 0.25f, 0.07f);

            // Add BaseModuleLighting
            BaseModuleLighting bml = lightSwitch.GetComponent<BaseModuleLighting>();
            if (bml == null)
                bml = lightSwitch.GetComponentInChildren<BaseModuleLighting>();
            if (bml == null)
                bml = lightSwitch.AddComponent<BaseModuleLighting>();

            // Add SkyApplier
            var sa = lightSwitch.AddComponent<SkyApplier>();
            sa.renderers = lightSwitchModel.GetComponentsInChildren<Renderer>(true);
            sa.anchorSky = Skies.Auto;

            // Add our custom MonoBehaviour controller
            var lightToggle = lightSwitch.AddComponent<BaseLightToggle>();

            // Store prefab
            loadedPrefab = lightSwitch;
        }

#if !NAUTILUS // SML Helper (Living Large update)
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
#endif
    }
}

// The crap in the #if below is now required due to how nautilus is implemented
#if NAUTILUS
namespace System.Runtime.CompilerServices
{
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class RequiredMemberAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            this.FeatureName = featureName;
        }

        public string FeatureName { get; }

        public bool IsOptional { get; set; }

        public const string RefStructs = "RefStructs";

        public const string RequiredMembers = "RequiredMembers";
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    [ExcludeFromCodeCoverage]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
}
#endif
