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
    class Program
    {
        static void Main(string[] args)
        {
            // init
            var error = EVRInitError.None;

            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
            if (error != EVRInitError.None) throw new Exception();

            OpenVR.GetGenericInterface(OpenVR.IVRInput_Version, ref error);
            if (error != EVRInitError.None) throw new Exception();

            var input = OpenVR.Input;

            VRActiveActionSet_t[] test = new VRActiveActionSet_t[1];
            var appError = OpenVR.Applications.AddApplicationManifest(Path.GetFullPath("C./app.vrmanifest"), false);
            var ioErr = OpenVR.Input.SetActionManifestPath(Path.GetFullPath("./actions.json"));
            //var ioErr = OpenVR.Input.SetActionManifestPath(Path.GetFullPath("./actions.json"));


            ulong actionHandle = 0;
            var inputError = input.GetActionHandle("/actions/lovense/in/Strength", ref actionHandle);

            ulong actionSetHandle = 0;
            inputError = input.GetActionSetHandle("/actions/lovense", ref actionSetHandle);

            VRActiveActionSet_t[] mActionSetArray = null;
            InputAnalogActionData_t mAction = new InputAnalogActionData_t();

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                var vrEvents = new List<VREvent_t>();
                var vrEvent = new VREvent_t();
                try
                {
                    while (OpenVR.System.PollNextEvent(ref vrEvent, (uint)Marshal.SizeOf(vrEvent)))
                    {
                        vrEvents.Add(vrEvent);
                    }
                }
                catch (Exception e)
                {
                }

                foreach (var e in vrEvents)
                {
                    var pid = e.data.process.pid;
                    if ((EVREventType)vrEvent.eventType != EVREventType.VREvent_None)
                    {
                        var name = Enum.GetName(typeof(EVREventType), e.eventType);
                        var message = $"[{pid}] {name}";
                        if (pid == 0) Console.WriteLine(message);
                        else if (name == null) Console.WriteLine(message);
                        else if (name.ToLower().Contains("fail")) Console.WriteLine(message);
                        else if (name.ToLower().Contains("error")) Console.WriteLine(message);
                        else if (name.ToLower().Contains("success")) Console.WriteLine(message);
                        else Console.WriteLine(message);
                    }
                }

                if (mActionSetArray == null)
                {
                    var actionSet = new VRActiveActionSet_t
                    {
                        ulActionSet = actionSetHandle,
                        ulRestrictedToDevice = OpenVR.k_ulInvalidActionSetHandle,
                        nPriority = 0
                    };
                    mActionSetArray = new VRActiveActionSet_t[] { actionSet };
                }
                var errorUAS = OpenVR.Input.UpdateActionState(mActionSetArray, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));


                
                GetAnalogInput(actionHandle, ref mAction, 0);
                

            }
            OpenVR.Shutdown();

        }

        private static void GetAnalogInput(ulong handle, ref InputAnalogActionData_t action, ulong restrict)
        {
            var size = (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t));
            var error = OpenVR.Input.GetAnalogActionData(handle, ref action, size, restrict);

            Console.WriteLine(action.y);
        }
    }
}
