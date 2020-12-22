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
            Controller controller = new Controller();
            controller.Setup();
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                controller.Loop();
            }

            controller.Shutdown();

        }
    }
}
