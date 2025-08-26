# WebRTC VideoReceiveSample System Diagram

## Theory of Operation

The VideoReceiveSample demonstrates a local WebRTC peer-to-peer connection where two RTCPeerConnection instances (pc1 and pc2) communicate with each other to simulate video and audio streaming.

```mermaid
graph TB
    subgraph "UI Controls"
        A[Call Button] --> B[Start Call Process]
        C[Hang Up Button] --> D[Cleanup Process]
        E[Add Tracks Button] --> F[Add Media Tracks]
        G[Remove Tracks Button] --> H[Remove Media Tracks]
        I[WebCam Toggle] --> J[Video Source Selection]
        K[Mic Toggle] --> L[Audio Source Selection]
    end

    subgraph "Media Capture"
        M[Camera/WebCam] --> N[Video Stream Track]
        O[Microphone/AudioClip] --> P[Audio Stream Track]
        N --> Q[Source Image Display]
        P --> R[Source Audio Output]
    end

    subgraph "WebRTC Peer Connection 1 (pc1 - Sender)"
        S[RTCPeerConnection pc1]
        S --> T[Add Video Track]
        S --> U[Add Audio Track]
        S --> V[Create Offer]
        S --> W[Set Local Description]
        S --> X[ICE Candidate Collection]
    end

    subgraph "WebRTC Peer Connection 2 (pc2 - Receiver)"
        Y[RTCPeerConnection pc2]
        Y --> Z[Set Remote Description]
        Y --> AA[Create Answer]
        Y --> BB[Set Local Description]
        Y --> CC[Receive Tracks]
        Y --> DD[ICE Candidate Collection]
    end

    subgraph "Media Output"
        EE[Received Video Track] --> FF[Receive Image Display]
        GG[Received Audio Track] --> HH[Receive Audio Output]
    end

    subgraph "ICE/STUN Process"
        II[STUN Server: stun.l.google.com:19302]
        II --> JJ[ICE Candidate Exchange]
        JJ --> KK[Connection Establishment]
    end

    %% Flow connections
    B --> M
    B --> O
    N --> T
    P --> U
    V --> Z
    W --> AA
    AA --> BB
    CC --> EE
    CC --> GG
    X --> DD
    DD --> X
    KK --> CC

    %% State management
    subgraph "Connection States"
        LL[New] --> MM[Checking]
        MM --> NN[Connected]
        NN --> OO[Completed]
        NN --> PP[Disconnected]
        PP --> QQ[Failed]
        NN --> RR[Closed]
    end

    style S fill:#e1f5fe
    style Y fill:#f3e5f5
    style N fill:#e8f5e8
    style P fill:#e8f5e8
    style EE fill:#fff3e0
    style GG fill:#fff3e0
```

## Key Components and Flow

### 1. Initialization Phase
- UI controls are set up with event listeners
- WebCam and microphone device lists are populated
- Event delegates are configured for peer connection callbacks

### 2. Call Establishment Phase
- **Media Capture**: Camera/WebCam and Microphone/AudioClip are captured
- **Peer Connection Creation**: Two RTCPeerConnection instances (pc1, pc2) are created
- **Track Addition**: Video and audio tracks are added to pc1
- **Offer/Answer Exchange**: 
  - pc1 creates an offer
  - pc1 sets local description with the offer
  - pc2 sets remote description with the offer
  - pc2 creates an answer
  - pc2 sets local description with the answer
  - pc1 sets remote description with the answer

### 3. ICE Candidate Exchange
- Both peer connections collect ICE candidates using STUN server
- Candidates are automatically exchanged between pc1 and pc2
- Connection state progresses: New ’ Checking ’ Connected ’ Completed

### 4. Media Streaming
- pc1 sends video and audio tracks to pc2
- pc2 receives tracks via OnTrack callback
- Received video is displayed in the receive image component
- Received audio is played through the receive audio component

### 5. Cleanup Phase
- All peer connections are disposed
- Media tracks are stopped and disposed
- UI controls are reset to initial state
- WebCam texture is stopped if active

## Technical Details
- Uses Unity's WebRTC package for peer-to-peer communication
- Supports both camera and WebCam video sources
- Supports both microphone and audio clip audio sources
- Implements proper graphics format conversion for WebCam textures
- Handles codec preferences for video streaming
- Provides comprehensive ICE connection state monitoring