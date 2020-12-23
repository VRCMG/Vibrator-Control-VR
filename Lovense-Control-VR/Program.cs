using Lovense;
using Lovense.Backends;
using Lovense.Toys;
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
            LovenseController lovense = LovenseController.WithConnectBackend(new Dictionary<string, string>() { ["host"] = "10.100.100.11" });
            
            Toy toy = lovense.GetToys()[0];
            //lovense.SendCommand(new CommandBuilder(toy).WithAction(LovenseAction.Vibrate).WithStrength(10).Build());
            //Console.WriteLine(toy.Id);
            
            Controller controller = new Controller();
            controller.Setup();
            int lastStrength = 0;
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                int strength = controller.Loop();
                if (strength != lastStrength)
                {
                    Console.WriteLine(strength);
                    Command cmd = new CommandBuilder(toy).WithStrength(strength).Build();
                    lovense.SendCommand(cmd);
                    lastStrength = strength;
                    // Thread.Sleep(500); //If you use the token api. Don't want to overload it
                }
            }

            controller.Shutdown();

        }
    }
}
