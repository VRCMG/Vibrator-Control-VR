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

            LovenseController lovense = LovenseController.WithTokenBackend(new Dictionary<string, string>() {["Token"]="TODO" });
            Toy toy = lovense.GetToys()[0];


            Controller controller = new Controller();
            controller.Setup();
            int lastStrength = 0;
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                int strength = controller.Loop();
                if (strength != lastStrength)
                {
                    Command cmd = new CommandBuilder().ForToy(toy).WithStrength(strength).Build();
                    lovense.SendCommand(cmd);
                    lastStrength = strength;
                    Thread.Sleep(500);
                }
            }

            controller.Shutdown();

        }
    }
}
