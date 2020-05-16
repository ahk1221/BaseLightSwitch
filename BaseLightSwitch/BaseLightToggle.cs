using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BaseLightSwitch
{
    public class BaseLightToggle : HandTarget, IHandTarget
    {
        public void OnHandClick(GUIHand hand)
        {
            if (!enabled) return;

            var subRoot = Player.main.GetCurrentSub();
            if (subRoot == null) return;

            var constructable = GetComponent<Constructable>();

            if (!constructable || constructable.constructed)
            {
                // Get current light state
                var isLightsOnField = typeof(SubRoot).GetField("subLightsOn", BindingFlags.Instance | BindingFlags.NonPublic);
                var isLightsOn = (bool)isLightsOnField.GetValue(subRoot);

                // Play sound depending on state
#pragma warning disable CS0618
                if (isLightsOn)
                    FMODUWE.PlayOneShot("event:/sub/cyclops/lights_off", MainCamera.camera.transform.position, 1f);
                else
                    FMODUWE.PlayOneShot("event:/sub/cyclops/lights_on", MainCamera.camera.transform.position, 1f);
#pragma warning restore CS0618

                // Update light state
                subRoot.ForceLightingState(!isLightsOn);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            var reticle = HandReticle.main;
            reticle.SetIcon(HandReticle.IconType.Hand, 1f);
            reticle.SetInteractText("ToggleLightsBase");
        }
    }
}
