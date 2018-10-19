using System;
using System.Collections;
using UnityEngine.XR;

namespace Utils.VR
{
    public static class VrUtils
    {
        public static bool IsInVr;

        public static IEnumerator SwitchMode(bool vr)
        {
            IsInVr = vr;

            var deviceName = vr ? "OpenVR" : "None";

            if (string.Compare(XRSettings.loadedDeviceName, deviceName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                XRSettings.LoadDeviceByName(deviceName);
                yield return null;
            }

            XRSettings.enabled = vr;
            // XRSettings.showDeviceView = !vr;
        }
    }
}