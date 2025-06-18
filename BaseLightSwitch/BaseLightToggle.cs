namespace BaseLightSwitch
{
    using System.IO;
    using System.Reflection;
    using System.Text;
    using UnityEngine;

    public class BaseLightToggle : HandTarget, IHandTarget
    {
        // Get light state field
        private static readonly FieldInfo _isLightsOnField = typeof(SubRoot).GetField("subLightsOn", BindingFlags.Instance | BindingFlags.NonPublic);

        public BaseLightToggle() { }

        /// <summary>Returns the SubRoot object (if light switch is in a base, the BaseRoot is casted into a SubRoot).</summary>
        public SubRoot GetSubRoot()
        {
            SubRoot subRoot = GetComponentInParent<SubRoot>(); // Try get SubRoot (if light switch is in a submarine)
            if (subRoot == null) subRoot = gameObject?.transform?.parent?.GetComponent<SubRoot>(); // Try get SubRoot from gameObject's parent (if light switch is in a submarine)
            if (subRoot == null) subRoot = GetComponentInParent<BaseRoot>(); // Try get SubRoot from BaseRoot (if light switch is in a base)
            if (subRoot == null) subRoot = gameObject?.transform?.parent?.GetComponent<BaseRoot>();  // Try get SubRoot from gameObject's parent BaseRoot (if light switch is in a base)
            return subRoot;
        }

        /// <summary>Gets called upon HandClick event.</summary>
        /// <param name="hand">The hand that triggered the click event.</param>
        public void OnHandClick(GUIHand hand)
        {
            if (!enabled) return;

            // Get light switch SubRoot
            var subRoot = GetSubRoot();
            if (subRoot == null) return; // Return if light switch is not in a base or in a submarine

            // Get light switch Constructable
            var constructable = GetComponent<Constructable>();
            if (constructable == null || !constructable.constructed) return; // Return if light switch has not been built

            // Get current light state
            var isLightsOn = (bool)_isLightsOnField.GetValue(subRoot);

            // Set new light state
            isLightsOn = !isLightsOn;
            subRoot.ForceLightingState(isLightsOn);

            // Play sound (depending on new light state). Scraped from : https://github.com/K07H/DecorationsMod/blob/master/Subnautica_AudioAssets.txt
            if (isLightsOn)
                FMODUWE.PlayOneShot(new FMODAsset() { id = "2103", path = "event:/sub/cyclops/lights_on", name = "5384ec29-f493-4ac1-9f74-2c0b14d61440", hideFlags = HideFlags.None }, MainCamera.camera.transform.position, 1f);
            else
                FMODUWE.PlayOneShot(new FMODAsset() { id = "2102", path = "event:/sub/cyclops/lights_off", name = "95b877e8-2ccd-451d-ab5f-fc654feab173", hideFlags = HideFlags.None }, MainCamera.camera.transform.position, 1f);
        }

        /// <summary>Gets called upon HandHover event.</summary>
        /// <param name="hand">The hand that triggered the hover event.</param>
        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            var reticle = HandReticle.main;
            reticle.SetIcon(HandReticle.IconType.Hand, 1f);
#if NAUTILUS
            reticle.SetText(HandReticle.TextType.Hand, "ToggleLightsBase", true, GameInput.Button.LeftHand);
#else // SML Helper (Living Large update)
            reticle.SetInteractText("ToggleLightsBase");
#endif
        }
    }
}
