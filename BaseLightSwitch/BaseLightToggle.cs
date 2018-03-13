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
                var isLightsOnField = typeof(SubRoot).GetField("subLightsOn", BindingFlags.Instance | BindingFlags.NonPublic);
                var isLightsOn = (bool)isLightsOnField.GetValue(subRoot);

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
