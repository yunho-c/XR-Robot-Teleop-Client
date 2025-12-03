using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// Debug script to check XR initialization status.
/// Attach to any GameObject in scene to see XR info in console.
/// </summary>
public class XRDebugInfo : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckAndForceXR());
    }
    
    System.Collections.IEnumerator CheckAndForceXR()
    {
        // Check if XR is enabled
        Debug.Log("=== XR DEBUG INFO ===");
        Debug.Log($"XR Enabled: {XRSettings.enabled}");
        Debug.Log($"XR Device: {XRSettings.loadedDeviceName}");
        Debug.Log($"XR Display: {XRSettings.isDeviceActive}");
        
        // Check XR Manager
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager != null)
        {
            Debug.Log($"XR Manager Active: {xrManager.isInitializationComplete}");
            Debug.Log($"Active Loader: {xrManager.activeLoader?.name ?? "None"}");
            
            // If XR is not initialized, provide helpful debug info
            if (!xrManager.isInitializationComplete)
            {
                Debug.Log("=== XR NOT INITIALIZED - DEBUGGING INFO ===");
                Debug.Log("XR Manager exists but initialization is not complete.");
                Debug.Log("This usually means:");
                Debug.Log("1. Quest Link is not active in headset");
                Debug.Log("2. OpenXR runtime is not set to Oculus");
                Debug.Log("3. XR Plugin Management settings are incorrect");
                Debug.Log("4. Unity is not detecting the XR device");
                
                // Check if we can manually start subsystems
                if (xrManager.activeLoader != null)
                {
                    Debug.Log($"Active loader found: {xrManager.activeLoader.name}");
                    Debug.Log("Attempting to start subsystems manually...");
                    xrManager.StartSubsystems();
                }
                else
                {
                    Debug.LogError("No active loader found - XR cannot initialize");
                }
                
                // Wait a moment
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            Debug.LogError("XR Manager is NULL - XR not initialized!");
        }
        
        // Wait a moment for initialization to complete
        yield return new WaitForSeconds(1f);
        
        // List all XR devices
        var devices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevices(devices);
        Debug.Log($"Connected XR Devices: {devices.Count}");
        foreach (var device in devices)
        {
            Debug.Log($"  - {device.name} ({device.role})");
        }
        
        // Check for HMD specifically
        var hmdDevices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesWithRole(InputDeviceRole.Generic, hmdDevices);
        Debug.Log($"HMD Devices: {hmdDevices.Count}");
        
        // Final status check
        Debug.Log($"Final Status - XR Enabled: {XRSettings.enabled}");
        Debug.Log($"Final Status - XR Device: {XRSettings.loadedDeviceName}");
        Debug.Log($"Final Status - XR Display: {XRSettings.isDeviceActive}");
    }
    
    void Update()
    {
        // Continuous check every 2 seconds
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"[{Time.time:F1}s] XR Active: {XRSettings.isDeviceActive}, Device: {XRSettings.loadedDeviceName}");
        }
    }
}


