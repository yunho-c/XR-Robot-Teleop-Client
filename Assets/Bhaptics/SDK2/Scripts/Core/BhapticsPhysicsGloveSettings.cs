using UnityEngine;

namespace Bhaptics.SDK2
{
    [CreateAssetMenu(fileName = "GloveHapticSettings", menuName = "Bhaptics/GloveHapticSettings")]
    public class BhapticsPhysicsGloveSettings : ScriptableObject
    {
        public enum HapticMode
        {
            Mode1 = 0,

            Mode2 = 1,

            Mode3 = 2,

            Mode4 = 3
        }



        public HapticMode hapticMode = HapticMode.Mode1;

        [Tooltip("Maximum value of the motor intensity [0, 100]")]
        [Range(0, 100)]
        public int motorIntensityMax = 50;

        [Tooltip("Minimum value of the motor intensity [0, 100]")]
        [Range(0, 100)]
        public int motorIntensityMin = 1;

        [Tooltip("Maximum value of relative velocity change (m/s)")]
        public float velocityChangeMax = 2.0f;

        [Tooltip("Minimum value of relative velocity change (m/s)")]
        public float velocityChangeMin = 0.2f;

        [Tooltip("After collision, motor strength decreases at a rate of exponential DecayRate after exponential DecayDelay seconds")]
        [Range(0f, 1f)]
        public float decayRate = 0.3f;

        [Tooltip("exponential Delay in seconds before motor strength starts to decrease after collision")]
        [Range(0f, 1f)]
        public float decayDelay = 0.5f;

        [Tooltip("Max value of distance difference between Master-Slave (0, 100] (cm)")]
        [Range(0f, 100f)]
        public float masterSlaveDistanceMax = 20f;
    }
}
