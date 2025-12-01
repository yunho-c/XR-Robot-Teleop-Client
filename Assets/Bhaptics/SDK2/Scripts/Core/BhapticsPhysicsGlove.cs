using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bhaptics.SDK2.Glove
{
    /// <summary>
    /// Handles haptic feedback for Bhaptics Physics Glove.
    /// </summary>
    public class BhapticsPhysicsGlove : MonoBehaviour
    {
        private const float MotorIntensityCorrectionWithDecay = 0.8f;
        private const float MotorIntensityCorrectionWithoutDecay = 0.5f;

        private static  BhapticsPhysicsGlove instance;
        public static BhapticsPhysicsGlove Instance 
        {
            get
            {
                return instance;
            }
        }

        [Header("Settings")]
        [SerializeField] private BhapticsPhysicsGloveSettings hapticSettings;
        public BhapticsPhysicsGloveSettings HapticSettings 
        {
            get 
            {
                return hapticSettings;
            }
            set 
            {
                hapticSettings = value;
            }
        }
        private Dictionary<string, float> lastEvent = new Dictionary<string, float>();


        public float HapticVolume = 1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            if (hapticSettings == null)
            {
                Debug.LogError("GloveHapticSettings is not assigned.");
            }
        }

        /// <summary>
        /// Changes the haptic mode.
        /// </summary>
        /// <param name="mode">The new haptic mode.</param>
        public void ChangeHapticMode(BhapticsPhysicsGloveSettings.HapticMode mode)
        {
            hapticSettings.hapticMode = mode;
        }

        /// <summary>
        /// Plays haptic feedback on a specified finger.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="intensity">Intensity of the haptic feedback.</param>
        private void PlayHaptic(PositionType position, int fingerIndex, float intensity)
        {
            var currentTime = Time.time * 1000;
            var key = $"{position}_{fingerIndex}".ToLower();

            if (!lastEvent.ContainsKey(key))
            {
                lastEvent[key] = -1;
            }

            var diff = currentTime - lastEvent[key];
            if (diff < 50)
            {
                return;
            }

            var dots = new int[6] { 0, 0, 0, 0, 0, 0 };
            dots[fingerIndex] = Mathf.RoundToInt(intensity * HapticVolume);

            if (position == PositionType.GloveL)
            {
                BhapticsLibrary.PlayMotors((int)PositionType.GloveL, dots, 100);
            }
            else if (position == PositionType.GloveR)
            {
                BhapticsLibrary.PlayMotors((int)PositionType.GloveR, dots, 100);
            }

            lastEvent[key] = currentTime;
        }

        #region EnterHaptic
        /// <summary>
        /// Sends enter haptic feedback.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendEnterHaptic(bool isLeft, int fingerIndex)
        {
            SendEnterHaptic(isLeft, fingerIndex, hapticSettings.velocityChangeMax * Vector3.forward);
        }
        /// <summary>
        /// Sends enter haptic feedback with collision.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="collision">Collision data.</param>
        public void SendEnterHaptic(bool isLeft, int fingerIndex, Collision collision)
        {
            SendEnterHaptic(isLeft, fingerIndex, collision.relativeVelocity);
        }

        /// <summary>
        /// Sends enter haptic feedback with velocity.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="velocity">Velocity data.</param>
        public void SendEnterHaptic(bool isLeft, int fingerIndex, Vector3 velocity)
        {
            SendEnterHaptic(isLeft ? PositionType.GloveL : PositionType.GloveR, fingerIndex, velocity);
        }

        /// <summary>
        /// Sends enter haptic feedback with position data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendEnterHaptic(PositionType position, int fingerIndex)
        {
            SendEnterHaptic(position, fingerIndex, hapticSettings.velocityChangeMax * Vector3.forward);
        }

        /// <summary>
        /// Sends enter haptic feedback with collision and position data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="collision">Collision data.</param>
        public void SendEnterHaptic(PositionType position, int fingerIndex, Collision collision)
        {
            SendEnterHaptic(position, fingerIndex, collision.relativeVelocity);
        }

        /// <summary>
        /// Sends enter haptic feedback with velocity and position data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="velocity">Velocity data.</param>
        public void SendEnterHaptic(PositionType position, int fingerIndex, Vector3 velocity)
        {
            float motorValue = 0f;

            switch (hapticSettings.hapticMode)
            {
                case BhapticsPhysicsGloveSettings.HapticMode.Mode1:
                case BhapticsPhysicsGloveSettings.HapticMode.Mode2:

                    Vector3 onEnterFingerVel = velocity;
                    motorValue = ((hapticSettings.motorIntensityMax - hapticSettings.motorIntensityMin) * (onEnterFingerVel.magnitude - hapticSettings.velocityChangeMin)) / (hapticSettings.velocityChangeMax - hapticSettings.velocityChangeMin) + hapticSettings.motorIntensityMin;
                    motorValue = Mathf.Clamp(motorValue, hapticSettings.motorIntensityMin, hapticSettings.motorIntensityMax * MotorIntensityCorrectionWithDecay);

                    break;

                case BhapticsPhysicsGloveSettings.HapticMode.Mode3:
                case BhapticsPhysicsGloveSettings.HapticMode.Mode4:

                    motorValue = hapticSettings.motorIntensityMax * MotorIntensityCorrectionWithoutDecay;

                    break;
            }

            PlayHaptic(position, fingerIndex, motorValue);
        }
        #endregion

        #region StayHaptic
        /// <summary>
        /// Sends stay haptic feedback.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendStayHaptic(bool isLeft, int fingerIndex)
        {
            SendStayHaptic(isLeft, fingerIndex, this.transform, this.transform);
        }

        /// <summary>
        /// Sends stay haptic feedback with transform data.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="slaveTransform">Slave transform data.</param>
        /// <param name="masterTransform">Master transform data.</param>
        public void SendStayHaptic(bool isLeft, int fingerIndex, Transform slaveTransform, Transform masterTransform)
        {
            SendStayHaptic(isLeft ? PositionType.GloveL : PositionType.GloveR, fingerIndex, slaveTransform, masterTransform);
        }

        /// <summary>
        /// Sends stay haptic feedback with position data.
        /// </summary>
        /// <param name="position">Position type (left or right glove).</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendStayHaptic(PositionType position, int fingerIndex)
        {
            SendStayHaptic(position, fingerIndex, this.transform, this.transform);
        }

        /// <summary>
        /// Sends stay haptic feedback with position and transform data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="slaveTransform">Slave transform data.</param>
        /// <param name="masterTransform">Master transform data.</param>
        public void SendStayHaptic(PositionType position, int fingerIndex, Transform slaveTransform, Transform masterTransform)
        {
            float motorValue = 0f;
            switch (hapticSettings.hapticMode)
            {
                case BhapticsPhysicsGloveSettings.HapticMode.Mode1:

                    motorValue = Mathf.Max(motorValue * Mathf.Pow(hapticSettings.decayRate, Time.fixedDeltaTime / hapticSettings.decayDelay), ((hapticSettings.motorIntensityMax - hapticSettings.motorIntensityMin) * Mathf.Pow((slaveTransform.position - masterTransform.position).magnitude, 2)) / Mathf.Pow(hapticSettings.masterSlaveDistanceMax / 100f, 2) + hapticSettings.motorIntensityMin);
                    motorValue = Mathf.Clamp(motorValue, hapticSettings.motorIntensityMin, hapticSettings.motorIntensityMax);

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode2:

                    motorValue = Mathf.Max(motorValue * Mathf.Pow(hapticSettings.decayRate, Time.fixedDeltaTime / hapticSettings.decayDelay), hapticSettings.motorIntensityMin);
                    motorValue = Mathf.Clamp(motorValue, hapticSettings.motorIntensityMin, hapticSettings.motorIntensityMax);

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode3:

                    motorValue = hapticSettings.motorIntensityMin;

                    break;
                case BhapticsPhysicsGloveSettings.HapticMode.Mode4:

                    motorValue = 0;

                    break;
            }

            PlayHaptic(position, fingerIndex, motorValue);
        }

        /// <summary>
        /// Sends stay haptic feedback with relative distance data.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="relativeDistance">Relative distance data.</param>
        public void SendStayHaptic(bool isLeft, int fingerIndex, float relativeDistance)
        {
            SendStayHaptic(isLeft ? PositionType.GloveL : PositionType.GloveR, fingerIndex, relativeDistance);
        }

        /// <summary>
        /// Sends stay haptic feedback with relative distance and position data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        /// <param name="relativeDistance">Relative distance data.</param>
        public void SendStayHaptic(PositionType position, int fingerIndex, float relativeDistance)
        {

            float motorValue = 0f;
            switch (hapticSettings.hapticMode)
            {
                case BhapticsPhysicsGloveSettings.HapticMode.Mode1:

                    motorValue = Mathf.Max(motorValue * Mathf.Pow(hapticSettings.decayRate, Time.fixedDeltaTime / hapticSettings.decayDelay), (hapticSettings.motorIntensityMax - hapticSettings.motorIntensityMin) * Mathf.Pow(relativeDistance, 2) / Mathf.Pow(hapticSettings.masterSlaveDistanceMax / 100f, 2) + hapticSettings.motorIntensityMin);
                    motorValue = Mathf.Clamp(motorValue, hapticSettings.motorIntensityMin, hapticSettings.motorIntensityMax);

                    break;

                case BhapticsPhysicsGloveSettings.HapticMode.Mode2:

                    motorValue = Mathf.Max(motorValue * Mathf.Pow(hapticSettings.decayRate, Time.fixedDeltaTime / hapticSettings.decayDelay), hapticSettings.motorIntensityMin);
                    motorValue = Mathf.Clamp(motorValue, hapticSettings.motorIntensityMin, hapticSettings.motorIntensityMax);

                    break;

                case BhapticsPhysicsGloveSettings.HapticMode.Mode3:

                    motorValue = hapticSettings.motorIntensityMin;

                    break;

                case BhapticsPhysicsGloveSettings.HapticMode.Mode4:

                    motorValue = 0;

                    break;
            }

            PlayHaptic(position, fingerIndex, motorValue);
        }
        #endregion

        #region ExitHaptic

        /// <summary>
        /// Sends Exit haptic feedback.
        /// </summary>
        /// <param name="isLeft">Indicates if the left glove is used.</param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendExitHaptic(bool isLeft, int fingerIndex)
        {
            SendExitHaptic(isLeft ? PositionType.GloveL : PositionType.GloveR, fingerIndex);
        }

        /// <summary>
        /// Sends Exit haptic feedback with position data.
        /// </summary>
        /// <param name="position">
        /// <para>type of device</para>
        /// <para> 8 = <see cref="PositionType.GloveL"/> || 9 = <see cref="PositionType.GloveR"/> </para>
        /// </param>
        /// <param name="fingerIndex">Index of the finger.
        /// <para>0 = ThumbTip / 1 = IndexTip / 2 = MiddleTip / 3 = RingTip / 4 = LittleTip</para>
        /// <para>5 = Wrist</para>
        /// </param>
        public void SendExitHaptic(PositionType position, int fingerIndex)
        {
            float motorValue = 0f;

            switch (hapticSettings.hapticMode)
            {
                case BhapticsPhysicsGloveSettings.HapticMode.Mode1:
                case BhapticsPhysicsGloveSettings.HapticMode.Mode2:
                case BhapticsPhysicsGloveSettings.HapticMode.Mode3:

                    motorValue = 0;

                    break;

                case BhapticsPhysicsGloveSettings.HapticMode.Mode4:

                    motorValue = hapticSettings.motorIntensityMin;

                    break;
            }

            PlayHaptic(position, fingerIndex, motorValue);
        }
        #endregion
    }

}