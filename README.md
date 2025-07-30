# XR-Robot-Teleop

`XR-Robot-Teleop` is a **robot teleoperation client for VR/AR headsets** built in Unity. It uses [Meta Movement SDK](https://developers.meta.com/horizon/documentation/unity/move-body-tracking/) to obtain upper body poses (via [inside-out body tracking](https://developers.meta.com/horizon/blog/inside-out-body-tracking-and-generative-legs/)) and sends it to a user-specified IP address via WebRTC. 

[`XR-Teleop-Server`](https://github.com/yunho-c/XR-Teleop-Server) is a companion project written in Python that receives the body pose data. You can define custom callback functions (which are invoked every time the data is received) to visualize body poses or integrate into a robotic simulator. 

## Instructions

### Installation

#### Option 1. Using Meta Quest Developer Hub

> This requires developer mode to be enabled on your Quest device. Refer to the instructions [here](https://developers.meta.com/horizon/documentation/native/android/mobile-device-setup/).

- Download the latest `.apk` file from [releases](https://github.com/yunho-c/XR-Robot-Teleop/releases).
- Connect your headset to computer (via USB-C cable).
- Open Meta Quest Developer Hub and the 'Devices' panel.
- Drag & drop the .apk file into the right side of the screen. 

### Option 2. Install from Meta App Lab

`TODO`: We have not yet uploaded the project to App Lab and will do so soon!

## Details

### Theory of Operation

```mermaid
graph TD
    subgraph "Unity Client (XR Headset)"
        A[Meta Movement SDK<br>/<br>MetaSourceDataProvider] -- Raw Pose --> B(BodyPoseProvider);
        B -- OnPoseUpdated Event<br>(C# PoseData) --> C(WebRTCController);
        B -- OnPoseUpdated Event<br>(C# PoseData) --> D(BodyPoseLogger);

        C -- Serializes PoseData --> E[Binary Data];
        E -- Sends via --> F(RTCDataChannel<br>body_pose);

        G[VR Camera Transform] -- Gets Orientation --> C;
        C -- Serializes Orientation --> H[JSON Data];
        H -- Sends via --> I(RTCDataChannel<br>camera);

        D -- Formats as JSON/String --> J[Debug Log];
    end

    subgraph "WebRTC Network"
        F -- Unreliable/Unordered --> K{Signaling &<br>Peer Connection};
        I -- Reliable/Ordered --> K;
    end

    subgraph "Python Server"
        K -- Binary Data --> L(XR-Teleop-Server);
        K -- JSON Data --> L;
        L -- Invokes --> M(User-Defined Callbacks<br>e.g., Robot Control, Visualization);
    end

    style A fill:#D4E1F5
    style G fill:#D4E1F5
    style J fill:#FFF2CC
    style M fill:#D5E8D4
```

### Hardware Compatibility

| Device | Supported | Notes |
| :--- | :---: | :--- |
| Meta Quest 3 | ✅ | fully supported |
| Meta Quest Pro | ⚠️ | untested |
| Apple Vision Pro | ❌ | not supported |

### Roadmap

[TODO: add roadmap link]

### Contribute

Please create an issue for feature requests and bug reports! PRs are welcome. 
