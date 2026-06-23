using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using Unity.Collections;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace B4TGames
{
    public class DisplayUtilities : MonoBehaviour
    {
        // The SpaceWarpFeature object that corresponds to the feature:
        // "Application SpaceWarp" in "Project Settings -> XR Plug-in Management -> OpenXR"
        private static SpaceWarpFeature spaceWarpFeature;
        private static bool spaceWarpFeatureEnabled;

        void Start()
        {
            // SetSpaceWarp(true);
            SetRefreshRate();
            SetFoveatedRendering();
        }

        public static void SetSpaceWarp(bool enabled)
        {
            // Check if SpaceWarp is enabled in Project Settings
            if (!spaceWarpFeature && OpenXRSettings.Instance)
            {
                spaceWarpFeature = OpenXRSettings.Instance.GetFeature<SpaceWarpFeature>();
            }
            
            if (spaceWarpFeature)
            {
                spaceWarpFeatureEnabled = spaceWarpFeature.enabled && enabled;

                var camera = Camera.main;
                if (spaceWarpFeatureEnabled)
                {
                    camera.depthTextureMode |= (DepthTextureMode.MotionVectors | DepthTextureMode.Depth);
                }
                else
                {
                    camera.depthTextureMode &= ~(DepthTextureMode.MotionVectors | DepthTextureMode.Depth);
                }
                
                SpaceWarpFeature.SetSpaceWarp(spaceWarpFeatureEnabled);
            }
        }

        private void SetRefreshRate()
        {
            if (XRGeneralSettings.Instance && XRGeneralSettings.Instance.Manager && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                var displaySubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();

                // Requires that you enable the Meta Quest Display Utilities feature in
                // Project Settings > XR Plug-in Management > OpenXR.
                if (displaySubsystem.TryGetSupportedDisplayRefreshRates(Allocator.Temp, out var refreshRates))
                {
                    var preferredRefreshRate = SystemInfo.deviceModel.Contains("Quest 3") ? 90.0f : 72.0f;
                    var success = false;
                    
                    foreach (var refreshRate in refreshRates)
                    {
                        if (refreshRate == preferredRefreshRate)
                        {
                            success = displaySubsystem.TryRequestDisplayRefreshRate(preferredRefreshRate);
                            break;
                        }
                    }

                    if (!success)
                    {
                        Debug.LogError("[DisplayUtilities] Failed to request display refresh rate: " + preferredRefreshRate);
                    }
                }
            }
        }

        private static void SetFoveatedRendering()
        {
            if (XRGeneralSettings.Instance && XRGeneralSettings.Instance.Manager && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                var level = 3f;
                var maxLevel = 3.0f;

                var displaySubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
                if (displaySubsystem != null)
                {
                    displaySubsystem.foveatedRenderingFlags = XRDisplaySubsystem.FoveatedRenderingFlags.GazeAllowed;
                    displaySubsystem.foveatedRenderingLevel = 0f;
                    displaySubsystem.foveatedRenderingLevel = level > 0f ? level / maxLevel : 0f;

                    Debug.Log("[DisplayUtilities] Foveated Rendering: " + displaySubsystem.foveatedRenderingLevel);
                }
            }
        }
    }
}