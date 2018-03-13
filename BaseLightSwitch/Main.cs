using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using SMLHelper;
using SMLHelper.Patchers;
using Oculus.Newtonsoft.Json;
using UnityEngine;
using System.IO;

namespace BaseLightSwitch
{
    public class Main
    {
        public static HarmonyInstance harmony;
        public static KeyCode keyCode;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create("com.ahk1221.baselightswitch");

            LoadConfig();

            var techType = TechTypePatcher.AddTechType("LightSwitch", "Light Switch", "A light switch.");

            // Load AssetBundle
            var ab = AssetBundle.LoadFromFile(@"./QMods/BaseLightSwitch/lightswitch.assets");

            // Load GameObject
            var lightSwitch = ab.LoadAsset<GameObject>("LightSwitch");
            Utility.AddBasicComponents(ref lightSwitch, "LightSwitch");

            var constructable = lightSwitch.AddComponent<Constructable>();
            constructable.allowedOnWall = true;
            constructable.allowedOnGround = false;
            constructable.allowedInSub = true;
            constructable.allowedInBase = true;
            constructable.allowedOnCeiling = false;
            constructable.allowedOutside = false;
            constructable.techType = techType;
            constructable.model = lightSwitch.FindChild("model");

            var bounds = lightSwitch.AddComponent<ConstructableBounds>();

            var techTag = lightSwitch.AddComponent<TechTag>();
            techTag.type = techType;

            var collider = lightSwitch.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.43f, 0.25f, 0.07f);

            var rb = lightSwitch.GetComponent<Rigidbody>();
            MonoBehaviour.DestroyImmediate(rb);

            var lightToggle = lightSwitch.AddComponent<BaseLightToggle>();

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab("LightSwitch", "Submarine/Build/LightSwitch", lightSwitch, techType));

            var techData = new TechDataHelper();
            techData._ingredients = new List<IngredientHelper>();
            techData._ingredients.Add(new IngredientHelper(TechType.Titanium, 2));
            techData._techType = techType;
            CraftDataPatcher.customTechData.Add(techType, techData);

            LanguagePatcher.customLines.Add("ToggleLightsBase", "Toggle Lights");

            CraftDataPatcher.customBuildables.Add(techType);

            var groups = typeof(CraftData).GetField("groups", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .GetValue(null) as Dictionary<TechGroup, Dictionary<TechCategory, List<TechType>>>;

            groups[TechGroup.InteriorModules][TechCategory.InteriorModule].Add(techType);

            CustomSpriteHandler.customSprites.Add(new CustomSprite(techType, ab.LoadAsset<Sprite>("LightSwitchSprite")));

            var gameObj = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(gameObj);

            gameObj.AddComponent<BaseLightToggle>();
        }

        public static void LoadConfig()
        {
            var filePath = @"./QMods/BaseLightSwitch/key.json";

            try
            {
                var json = JsonConvert.DeserializeObject<KeyConfig>(filePath);
                var key = Enum.Parse(typeof(KeyCode), json.key);
                keyCode = (KeyCode)key;
            }
            catch(Exception e)
            {
                var json = JsonConvert.SerializeObject(new KeyConfig() { key = "J" });
                File.WriteAllText(filePath, json);

                keyCode = KeyCode.J;
            }
        }
    }
}
