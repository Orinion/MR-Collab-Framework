# MR-Collab-Framework

Classes that support Unity MR Collab Apps

<p align="center">
  <img height="250" alt="AR" src="https://user-images.githubusercontent.com/12730894/114381019-f699d180-9b8a-11eb-9b9b-59cd3dd96f6a.png">
  <img height="250" alt="VR" src="https://user-images.githubusercontent.com/12730894/114381048-fbf71c00-9b8a-11eb-8bb7-4b2057d1d091.png">
</p>

## feature overview

- avatar system
- mark realworld objects
- share spatial mesh
- syncronize data
- display task

## Demo

You can see the classes in action in the demo.

https://user-images.githubusercontent.com/12730894/116037627-aaf62600-a668-11eb-918a-65308bfd184b.mp4

The demo is a pc maintenance scenario, where the goal is to change the hard drive of a computer.
You can find a download for the AR and VR unity project [here](https://github.com/Orinion/MR-Collab-Framework/releases/tag/DEMO)

## class overview

The classes are sorted into [shared](/Shared/) classes that are used by both applications and [AR](/AR/) and [VR](/VR/) classes that are only used by one of the applications.

### Data Sending

To enable communication between the AR and VR application, we provide the `LocalDataSource.cs` basecalass that can send and recieve serialized objects using a Web-RTC DataChannel. 

### Marker System

To enable the VR user to place markers in the world of the AR user, we created the `MarkerSyncronizer.cs` that syncronizes them between the applications. The VR app then uses the `MarkerSystem.cs` to enable the VR user to place new markers. The `SteamVRController.cs` handles the SteamVR inputs. The `MarkerPlaceMachine.cs` class allows the AR user to mark the position of the machine that requires maintenance.

### Spatial System

To share the Spatial mapping of the environment of the AR user with the VR user, we use the `CustomObserver.cs` to send, and the `MeshReciever.cs` to recieve and display the mesh to the VR user.

## installation

### requirements

- [Steam VR](https://store.steampowered.com/about/)
- [Unity Engine 2018.4.26f1](https://unity3d.com/get-unity/download/archive)
- [Windows SDK 18362+](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk/)
- [NodeJS](https://nodejs.org/en/download/)
- [node-dss](https://github.com/bengreenier/node-dss)

### setting up new unity project

- add the [SteamVR](https://valvesoftware.github.io/steamvr_unity_plugin/) plugin in your VR project
- add the [MRTK](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/mrtk-getting-started) in your AR project
- add the [MR-WebRTC](https://microsoft.github.io/MixedReality-WebRTC/)  unity library to both projects.

## Licenses

| Work | Author | License |
|---|---|---|
| MRTK-Unity |Microsoft| MIT|
|MR-WebRTC|Microsoft|MIT|
|SteamVR Plugin| ValveSoftware|BSD 3|
|MeshSerializer|Markus G??bel (Bunny83)|MIT|
|Low Poly Head|Zypheos|CC Attribution|
|Hand (low poly)|scibbletoad|CC Attribution|
