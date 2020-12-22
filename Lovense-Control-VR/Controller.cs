using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace Lovense_Control_VR
{
    class Controller
    {
        private InputAnalogActionData_t action;
        private ulong actionHandle;
        private ulong actionSetHandle;
        private VRActiveActionSet_t[] actionSetArray;

        public void Setup()
        {
            var error = EVRInitError.None;

            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);

            if (error != EVRInitError.None) throw new Exception();

            OpenVR.GetGenericInterface(OpenVR.IVRInput_Version, ref error);
            if (error != EVRInitError.None) throw new Exception();

            var input = OpenVR.Input;

            var appError = OpenVR.Applications.AddApplicationManifest(Path.GetFullPath("./app.vrmanifest"), false);
            var ioErr = OpenVR.Input.SetActionManifestPath(Path.GetFullPath("./actions.json"));

            //somehow invalid
            //if (appError != EVRApplicationError.None) throw new Exception();
            if (ioErr != EVRInputError.None) throw new Exception();

            actionHandle = 0;
            actionSetHandle = 0;

            ioErr = input.GetActionHandle("/actions/lovense/in/Strength", ref actionHandle);
            if (ioErr != EVRInputError.None) throw new Exception();

            ioErr = input.GetActionSetHandle("/actions/lovense", ref actionSetHandle);
            if (ioErr != EVRInputError.None) throw new Exception();


            action = new InputAnalogActionData_t();
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

        public void Loop()
        {
            var ioErr = OpenVR.Input.UpdateActionState(actionSetArray, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));
            if (ioErr != EVRInputError.None) throw new Exception();

            GetAnalogInput(actionHandle, ref action, 0);

            float value = action.y;

            int level = Utils.GetLovenseLevel(value, -0.8f, 0.5f);

            Console.WriteLine($"currentValue: {value} LovenseLevel:{level}");
        }

        private void GetAnalogInput(ulong handle, ref InputAnalogActionData_t action, ulong restrict)
        {
            var size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
            var error = OpenVR.Input.GetAnalogActionData(handle, ref action, size, restrict);
            if (error != EVRInputError.None) throw new Exception();
            
        }

    }
}
