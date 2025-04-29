This project is a Unity-based depth perception testing tool designed for environmental manipulation across scenes. 

To load this project into Unity, download Unity version 6000.0.40f1 and clone the repository into a folder. Ensure git lfs is downloaded and tracking '.fbx' files, then add the project to the Unity hub from disk. It can then be loaded and shown in the Unity editor.  

To run in VR, ensure the headset used is supported by SteamVR. Connect the headset to the computer running the project, and in project settings under 'OpenXR', check the controllers are enabled in the interaction profies, that the play mode is set to 'SteamVR', and that the 'XR Interaction Toolkit' section has 'Use XR Device Simulator in scenes' disabled. When play is started, the application should be displayed in the headset. 

The untextured scene creator script is found under 'Assets/Editor', while the data analysis and scene management scripts are found under 'Assets/SceneManagement'. 