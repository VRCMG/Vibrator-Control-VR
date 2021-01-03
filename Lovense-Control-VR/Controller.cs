using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Valve.VR;

namespace Lovense_Control_VR
{
    class Controller
    {
        private ulong actionHandleStrength1;
        private ulong actionHandleStrength2;
        private ulong actionHandleMode;


        private InputAnalogActionData_t actionStrength1;
        private InputAnalogActionData_t actionStrength2;
        private InputDigitalActionData_t actionMode;


        private ulong actionSetHandle;
        private VRActiveActionSet_t[] actionSetArray;

        public int level1;
        public int level2;
        public bool modePressed;

        public void Setup()
        {
            var error = EVRInitError.None;

            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);

            if (error != EVRInitError.None) throw new Exception();

            OpenVR.GetGenericInterface(OpenVR.IVRInput_Version, ref error);
            if (error != EVRInitError.None) throw new Exception();

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None) throw new Exception();

            var input = OpenVR.Input;

            var appError = OpenVR.Applications.AddApplicationManifest(Path.GetFullPath("./app.vrmanifest"), false);
            var ioErr = OpenVR.Input.SetActionManifestPath(Path.GetFullPath("./actions.json"));

            //somehow invalid
            //if (appError != EVRApplicationError.None) throw new Exception();
            if (ioErr != EVRInputError.None) throw new Exception();

            ioErr = input.GetActionHandle("/actions/lovense/in/Strength", ref actionHandleStrength1); if (ioErr != EVRInputError.None) throw new Exception();
            ioErr = input.GetActionHandle("/actions/lovense/in/Strength2", ref actionHandleStrength2); if (ioErr != EVRInputError.None) throw new Exception();
            ioErr = input.GetActionHandle("/actions/lovense/in/SwitchMode", ref actionHandleMode); if (ioErr != EVRInputError.None) throw new Exception();

            ioErr = input.GetActionSetHandle("/actions/lovense", ref actionSetHandle);
            if (ioErr != EVRInputError.None) throw new Exception();


            actionStrength1 = new InputAnalogActionData_t();
            actionStrength2 = new InputAnalogActionData_t();
            actionMode = new InputDigitalActionData_t();
            var actionSet = new VRActiveActionSet_t
            {
                ulActionSet = actionSetHandle,
                ulRestrictedToDevice = OpenVR.k_ulInvalidActionSetHandle,
                nPriority = 0
            };
            actionSetArray = new VRActiveActionSet_t[] { actionSet };
        }

        internal void Shutdown()
        {
            OpenVR.Shutdown();
        }

        public void RefreshValues()
        {
            var ioErr = OpenVR.Input.UpdateActionState(actionSetArray, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));
            if (ioErr != EVRInputError.None) throw new Exception();
            
            GetAnalogInput(actionHandleStrength1, ref actionStrength1, 0);
            GetAnalogInput(actionHandleStrength2, ref actionStrength2, 0);

            GetDigitalInput(actionHandleMode, ref actionMode, 0);

            float value1 = actionStrength1.y;
            float value2 = actionStrength2.y;

            modePressed = actionMode.bChanged && actionMode.bState;

            level1 = Utils.GetLovenseLevel(value1, -0.8f, 0.5f);
            level2 = Utils.GetLovenseLevel(value2, -0.8f, 0.5f);

            Console.WriteLine($"currentValues: ({value1}, {value2})LovenseLevel: ({level1}, {level2})  ModeKey: {modePressed}");

        }

        private void GetAnalogInput(ulong handle, ref InputAnalogActionData_t action, ulong restrict)
        {
            var size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
            var error = OpenVR.Input.GetAnalogActionData(handle, ref action, size, restrict);
            if (error != EVRInputError.None) throw new Exception();
        }
        private void GetDigitalInput(ulong handle, ref InputDigitalActionData_t action, ulong restrict)
        {
            var size = (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t));
            var error = OpenVR.Input.GetDigitalActionData(handle, ref action, size, restrict);
            if (error != EVRInputError.None) throw new Exception();
        }

        public void DisplayOverlay(string image, int duration)
        {
            var overlay = OpenVR.Overlay;

            ulong overlayHandle = 0;

            overlay.CreateOverlay("er1807.overlay", "VibratorOverlay", ref overlayHandle);

            overlay.SetOverlayFromFile(overlayHandle, Path.GetFullPath($"./{image}"));

            overlay.SetOverlayWidthInMeters(overlayHandle, 0.2f);
            HmdMatrix34_t transform = new HmdMatrix34_t();
            transform.m0 = 1.0f;
            transform.m1 = 0.0f; 
            transform.m2 = 0.0f;
            transform.m3 = 0.12f;
            transform.m4 = 0.0f; 
            transform.m5 = 1.0f;
            transform.m6 = 0.0f;
            transform.m7 = 0.08f;
            transform.m8 = 0.0f;
            transform.m9 = 0.0f; 
            transform.m10 = 1.0f;
            transform.m11 = -0.3f;
            overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, OpenVR.k_unTrackedDeviceIndex_Hmd, ref transform);
            overlay.ShowOverlay(overlayHandle);

            new Thread(() =>
            {
                Thread.Sleep(duration);
                overlay.DestroyOverlay(overlayHandle);
            }).Start();


        }

    }
}
