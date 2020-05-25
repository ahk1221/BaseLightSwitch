namespace BaseLightSwitch
{
    using UnityEngine;

    /// <summary>Small component attached to bases/cyclops to restore their lights state.</summary>
    public class SubRootRestoreLightsState : MonoBehaviour
    {
        public bool IsLightsOn = true;

        /// <summary>Gets called when this MonoBehaviour wakes up.</summary>
        public void Awake()
        {
            if (enabled)
                Invoke("RestoreLightState", 3.0f); // We add a small delay before restoring lights state because the cyclops needs few frames to complete its initialization
        }

        /// <summary>This function gets called by <see cref="Awake"/> method. It restores light state of current base or submarine.</summary>
        public void RestoreLightState()
        {
            if (!enabled)
                return;

            // Get current base/cyclops and restore its lights state
            SubRoot subRoot = GetComponent<SubRoot>();
            if (subRoot != null)
                subRoot.ForceLightingState(this.IsLightsOn);
        }
    }
}
