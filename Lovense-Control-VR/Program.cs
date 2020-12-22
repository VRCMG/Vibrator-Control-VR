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
    //Based on https://github.com/BOLL7708/OpenVRInputTest
    class Program
    {
        static void Main(string[] args)
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

            ulong actionHandle = 0;
            ulong actionSetHandle = 0;

            ioErr = input.GetActionHandle("/actions/lovense/in/Strength", ref actionHandle);
            if (ioErr != EVRInputError.None) throw new Exception();

            ioErr = input.GetActionSetHandle("/actions/lovense", ref actionSetHandle);
            if (ioErr != EVRInputError.None) throw new Exception();


            InputAnalogActionData_t action = new InputAnalogActionData_t();
            var actionSet = new VRActiveActionSet_t
            {
                ulActionSet = actionSetHandle,
                ulRestrictedToDevice = OpenVR.k_ulInvalidActionSetHandle,
                nPriority = 0
            };
            VRActiveActionSet_t[] mActionSetArray = new VRActiveActionSet_t[] { actionSet };



            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                
                ioErr = OpenVR.Input.UpdateActionState(mActionSetArray, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));
                if (ioErr != EVRInputError.None) throw new Exception();

                GetAnalogInput(actionHandle, ref action, 0);

                float value = action.y;

                int level = GetLovenseLevel(value, -0.8f, 0.5f);

                Console.WriteLine($"currentValue: {value} LovenseLevel:{level}");
            }
            OpenVR.Shutdown();

        }

        private static int GetLovenseLevel(float value, float min, float max)
        {
            if (value == 0) return 0;
            //its hard to reach a vcalue of 1 on the upper limit. the the lower limit is easier reachable
            float newValue = Lerp(min, max, 0, 20, value);
            int intValue = (int)Math.Round(newValue);

            if (intValue < 0) intValue = 0;
            if (intValue > 20) intValue = 20;

            return intValue;
        }

        //https://forum.unity.com/threads/mapping-or-scaling-values-to-a-new-range.180090/
        public static float Lerp(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
        {

            float OldRange = (OldMax - OldMin);
            float NewRange = (NewMax - NewMin);
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

            return (NewValue);
        }

        private static void GetAnalogInput(ulong handle, ref InputAnalogActionData_t action, ulong restrict)
        {
            var size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
            var error = OpenVR.Input.GetAnalogActionData(handle, ref action, size, restrict);
            if (error != EVRInputError.None) throw new Exception();

        }
    }
}
