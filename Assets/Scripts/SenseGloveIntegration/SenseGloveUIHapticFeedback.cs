using UnityEngine;
using SG;
using Oculus.Interaction;
using System.Collections.Generic;

/// <summary>
/// Provides haptic feedback via SenseGlove when interacting with UI elements using Meta Interaction SDK.
/// Supports vibration and force feedback for various interaction types.
/// </summary>
public class SenseGloveUIHapticFeedback : MonoBehaviour
{
    [Header("Hardware")]
    [Tooltip("The SenseGlove haptic hardware device")]
    public SG_HapticGlove hapticGlove;

    [Header("Haptic Settings - Vibration")]
    [Tooltip("Vibration intensity for hover (0-1)")]
    [Range(0f, 1f)]
    public float hoverVibrationIntensity = 0.2f;

    [Tooltip("Vibration intensity for button press (0-1)")]
    [Range(0f, 1f)]
    public float pressVibrationIntensity = 0.7f;

    [Tooltip("Vibration intensity for button release (0-1)")]
    [Range(0f, 1f)]
    public float releaseVibrationIntensity = 0.4f;

    [Tooltip("Duration of press haptic feedback (seconds)")]
    public float pressDuration = 0.1f;

    [Tooltip("Frequency for vibration feedback (Hz)")]
    public float vibrationFrequency = 100f;

    [Header("Haptic Settings - Force Feedback")]
    [Tooltip("Enable force feedback (finger resistance) for UI interactions")]
    public bool enableForceFeedback = true;

    [Tooltip("Force feedback level when pressing against UI surface (0-1)")]
    [Range(0f, 1f)]
    public float surfaceForceLevel = 0.5f;

    [Tooltip("Force feedback level when button is fully pressed (0-1)")]
    [Range(0f, 1f)]
    public float buttonPressForceLevel = 0.8f;

    [Header("Interactor References")]
    [Tooltip("Poke interactor for this hand (for button/UI poking)")]
    public PokeInteractor pokeInteractor;

    [Tooltip("Ray interactor for this hand (for distant UI interaction)")]
    public RayInteractor rayInteractor;

    [Tooltip("Grab interactor for this hand (for slider/handle manipulation)")]
    public GrabInteractor grabInteractor;

    [Header("Debug")]
    public bool debugLogging = false;

    // Track current interaction state
    private HashSet<IInteractable> _currentlyHovering = new HashSet<IInteractable>();
    private HashSet<IInteractable> _currentlySelecting = new HashSet<IInteractable>();
    private bool _isApplyingForceFeedback = false;
    private float[] _forceFeedbackLevels = new float[5]; // Per-finger force levels

    void Start()
    {
        if (hapticGlove == null)
        {
            Debug.LogError($"[{name}] SenseGloveUIHapticFeedback requires a SG_HapticGlove reference!");
            this.enabled = false;
            return;
        }

        // Subscribe to interactor events
        if (pokeInteractor != null)
        {
            pokeInteractor.WhenInteractableSet.Action += OnPokeInteractableSet;
            pokeInteractor.WhenInteractableUnset.Action += OnPokeInteractableUnset;
            pokeInteractor.WhenInteractableSelected.Action += OnPokeInteractableSelected;
            pokeInteractor.WhenInteractableUnselected.Action += OnPokeInteractableUnselected;
        }

        if (rayInteractor != null)
        {
            rayInteractor.WhenInteractableSet.Action += OnRayInteractableSet;
            rayInteractor.WhenInteractableUnset.Action += OnRayInteractableUnset;
            rayInteractor.WhenInteractableSelected.Action += OnRayInteractableSelected;
            rayInteractor.WhenInteractableUnselected.Action += OnRayInteractableUnselected;
        }

        if (grabInteractor != null)
        {
            grabInteractor.WhenInteractableSet.Action += OnGrabInteractableSet;
            grabInteractor.WhenInteractableUnset.Action += OnGrabInteractableUnset;
            grabInteractor.WhenInteractableSelected.Action += OnGrabInteractableSelected;
            grabInteractor.WhenInteractableUnselected.Action += OnGrabInteractableUnselected;
        }

        if (debugLogging)
        {
            Debug.Log($"[{name}] SenseGlove UI Haptic Feedback initialized");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (pokeInteractor != null)
        {
            pokeInteractor.WhenInteractableSet.Action -= OnPokeInteractableSet;
            pokeInteractor.WhenInteractableUnset.Action -= OnPokeInteractableUnset;
            pokeInteractor.WhenInteractableSelected.Action -= OnPokeInteractableSelected;
            pokeInteractor.WhenInteractableUnselected.Action -= OnPokeInteractableUnselected;
        }

        if (rayInteractor != null)
        {
            rayInteractor.WhenInteractableSet.Action -= OnRayInteractableSet;
            rayInteractor.WhenInteractableUnset.Action -= OnRayInteractableUnset;
            rayInteractor.WhenInteractableSelected.Action -= OnRayInteractableSelected;
            rayInteractor.WhenInteractableUnselected.Action -= OnRayInteractableUnselected;
        }

        if (grabInteractor != null)
        {
            grabInteractor.WhenInteractableSet.Action -= OnGrabInteractableSet;
            grabInteractor.WhenInteractableUnset.Action -= OnGrabInteractableUnset;
            grabInteractor.WhenInteractableSelected.Action -= OnGrabInteractableSelected;
            grabInteractor.WhenInteractableUnselected.Action -= OnGrabInteractableUnselected;
        }
    }

    void Update()
    {
        // Apply continuous force feedback if UI surface is being touched
        if (enableForceFeedback && _isApplyingForceFeedback)
        {
            ApplyContinuousForceFeedback();
        }
    }

    #region Poke Interactor Events

    private void OnPokeInteractableSet(IInteractable interactable)
    {
        _currentlyHovering.Add(interactable);
        SendHoverVibration();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Poke hover started on {interactable}");
        }
    }

    private void OnPokeInteractableUnset(IInteractable interactable)
    {
        _currentlyHovering.Remove(interactable);
        StopForceFeedback();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Poke hover ended on {interactable}");
        }
    }

    private void OnPokeInteractableSelected(IInteractable interactable)
    {
        _currentlySelecting.Add(interactable);
        SendPressVibration();
        StartForceFeedback(buttonPressForceLevel);
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Poke select on {interactable}");
        }
    }

    private void OnPokeInteractableUnselected(IInteractable interactable)
    {
        _currentlySelecting.Remove(interactable);
        SendReleaseVibration();
        StopForceFeedback();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Poke unselect on {interactable}");
        }
    }

    #endregion

    #region Ray Interactor Events

    private void OnRayInteractableSet(IInteractable interactable)
    {
        _currentlyHovering.Add(interactable);
        SendHoverVibration();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Ray hover started on {interactable}");
        }
    }

    private void OnRayInteractableUnset(IInteractable interactable)
    {
        _currentlyHovering.Remove(interactable);
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Ray hover ended on {interactable}");
        }
    }

    private void OnRayInteractableSelected(IInteractable interactable)
    {
        _currentlySelecting.Add(interactable);
        SendPressVibration();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Ray select on {interactable}");
        }
    }

    private void OnRayInteractableUnselected(IInteractable interactable)
    {
        _currentlySelecting.Remove(interactable);
        SendReleaseVibration();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Ray unselect on {interactable}");
        }
    }

    #endregion

    #region Grab Interactor Events

    private void OnGrabInteractableSet(IInteractable interactable)
    {
        _currentlyHovering.Add(interactable);
        SendHoverVibration();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Grab hover started on {interactable}");
        }
    }

    private void OnGrabInteractableUnset(IInteractable interactable)
    {
        _currentlyHovering.Remove(interactable);
        StopForceFeedback();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Grab hover ended on {interactable}");
        }
    }

    private void OnGrabInteractableSelected(IInteractable interactable)
    {
        _currentlySelecting.Add(interactable);
        SendPressVibration();
        StartForceFeedback(surfaceForceLevel);
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Grab select on {interactable}");
        }
    }

    private void OnGrabInteractableUnselected(IInteractable interactable)
    {
        _currentlySelecting.Remove(interactable);
        SendReleaseVibration();
        StopForceFeedback();
        
        if (debugLogging)
        {
            Debug.Log($"[{name}] Grab unselect on {interactable}");
        }
    }

    #endregion

    #region Haptic Output Methods

    private void SendHoverVibration()
    {
        if (hapticGlove != null && hoverVibrationIntensity > 0)
        {
            // Light vibration on index finger for hover
            hapticGlove.SendVibrationCmd(
                VibrationLocation.Index_Tip,
                hoverVibrationIntensity,
                0.05f, // Short duration
                vibrationFrequency
            );
        }
    }

    private void SendPressVibration()
    {
        if (hapticGlove != null && pressVibrationIntensity > 0)
        {
            // Stronger vibration on index finger for press
            hapticGlove.SendVibrationCmd(
                VibrationLocation.Index_Tip,
                pressVibrationIntensity,
                pressDuration,
                vibrationFrequency
            );

            // Also send palm vibration for stronger feedback
            hapticGlove.SendVibrationCmd(
                VibrationLocation.Palm_IndexSide,
                pressVibrationIntensity * 0.7f,
                pressDuration,
                vibrationFrequency
            );
        }
    }

    private void SendReleaseVibration()
    {
        if (hapticGlove != null && releaseVibrationIntensity > 0)
        {
            // Medium vibration on release
            hapticGlove.SendVibrationCmd(
                VibrationLocation.Index_Tip,
                releaseVibrationIntensity,
                pressDuration * 0.5f,
                vibrationFrequency
            );
        }
    }

    private void StartForceFeedback(float forceLevel)
    {
        if (!enableForceFeedback || hapticGlove == null)
            return;

        _isApplyingForceFeedback = true;
        
        // Apply force primarily to index finger (typical UI interaction finger)
        _forceFeedbackLevels[(int)SGCore.Finger.Thumb] = forceLevel * 0.3f;
        _forceFeedbackLevels[(int)SGCore.Finger.Index] = forceLevel;
        _forceFeedbackLevels[(int)SGCore.Finger.Middle] = forceLevel * 0.5f;
        _forceFeedbackLevels[(int)SGCore.Finger.Ring] = forceLevel * 0.3f;
        _forceFeedbackLevels[(int)SGCore.Finger.Pinky] = forceLevel * 0.2f;
    }

    private void ApplyContinuousForceFeedback()
    {
        if (hapticGlove != null)
        {
            // Queue force feedback levels for Nova 2
            hapticGlove.QueueFFBCmd(_forceFeedbackLevels);
        }
    }

    private void StopForceFeedback()
    {
        if (!enableForceFeedback || !_isApplyingForceFeedback)
            return;

        _isApplyingForceFeedback = false;
        
        // Reset all force levels to zero
        for (int i = 0; i < _forceFeedbackLevels.Length; i++)
        {
            _forceFeedbackLevels[i] = 0f;
        }

        if (hapticGlove != null)
        {
            hapticGlove.QueueFFBCmd(_forceFeedbackLevels);
        }
    }

    /// <summary>
    /// Public method to trigger custom haptic feedback from external scripts.
    /// </summary>
    /// <param name="intensity">Vibration intensity (0-1)</param>
    /// <param name="duration">Duration in seconds</param>
    /// <param name="location">Vibration location on the hand</param>
    public void TriggerCustomVibration(float intensity, float duration, VibrationLocation location = VibrationLocation.Index_Tip)
    {
        if (hapticGlove != null)
        {
            hapticGlove.SendVibrationCmd(location, intensity, duration, vibrationFrequency);
        }
    }

    /// <summary>
    /// Public method to set force feedback levels directly.
    /// </summary>
    /// <param name="levels">Array of 5 force levels (0-1) for each finger (thumb to pinky)</param>
    public void SetForceFeedbackLevels(float[] levels)
    {
        if (levels != null && levels.Length == 5)
        {
            _forceFeedbackLevels = levels;
            _isApplyingForceFeedback = true;
        }
    }

    #endregion
}

