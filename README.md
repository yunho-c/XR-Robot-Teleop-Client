# XR-Robot-Teleop-Client

[`XR-Robot-Teleop-Client`](https://github.com/yunho-c/XR-Robot-Teleop-Client) is a **robot teleoperation client for VR/AR headsets** built in Unity. It uses [Meta Movement SDK](https://developers.meta.com/horizon/documentation/unity/move-body-tracking/) to obtain upper body poses (via [inside-out body tracking](https://developers.meta.com/horizon/blog/inside-out-body-tracking-and-generative-legs/)) and sends it to a user-specified IP address via WebRTC. 

[`XR-Robot-Teleop-Server`](https://github.com/yunho-c/XR-Robot-Teleop-Server) is a companion project written in Python that receives the body pose data. You can define custom callback functions (which are invoked every time the data is received) to visualize body poses or integrate into a robotic simulator. 

[TODO: video, GIF, or image]

## Instructions

### Installation

#### Option 1. Using Meta Quest Developer Hub

> This requires developer mode to be enabled on your Quest device. Refer to the instructions [here](https://developers.meta.com/horizon/documentation/native/android/mobile-device-setup/).

- Download the latest `.apk` file from [releases](https://github.com/yunho-c/XR-Robot-Teleop-Client/releases).
- Connect your headset to computer (via USB-C).
- Open [Meta Quest Developer Hub](https://developers.meta.com/horizon/documentation/unity/ts-mqdh/) and click the 'Devices' tab.
- Drag & drop the `.apk` file into the right-hand-side of the screen. 

#### Option 2. Install from Meta App Lab

`TODO`: We have not yet uploaded the project to App Lab — we will do so soon!

### Usage

- Obtain the IP address of your computer.
  - If the headset and PC are in a common network (e.g., same Wi-Fi router, university network, ...), you can generally use the local IP address which you can find by running `ipconfig`.
  - If the headset and PC are in different networks, you will need to perform the routine process (of setting up port-forwarding, opening firewall at a specific port, obtaining public IP, ...).
    - If you are unfamiliar with this, `Tailscale` is highly recommended—it greatly simplifies this process while providing rigorous security.
- Setup [`XR-Robot-Teleop-Server`](https://github.com/yunho-c/XR-Robot-Teleop-Server) Python package and clone its repository.
- Run one of its example scripts (e.g., `360-unity-teleop.py`).
  - Wait until it finishes loading and says: `TODO`.
- Open `XR-Robot-Teleop-Client` and enter the IP address of the PC, then press the connect icon.
- Voila!

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
        K -- Binary Data --> L(XR-Robot-Teleop-Server);
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

Please create an issue for feature requests and bug reports! PRs are always welcome. 

## Collaboration

### Setup

Typcial location of UnityYAMLMerge:

- Windows: `TODO`
- macOS: `~/Documents/Unity/<version>/Contents/Tools/UnityYAMLMerge`
- Linux: `TODO`

```bash
git config merge.tool unityyamlmerge

git config mergetool.unityyamlmerge.cmd '<UnityYAMLMerge_path> merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'

git config mergetool.unityyamlmerge.trustExitCode true
```

