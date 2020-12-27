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
            LovenseController lovense = null;
            Console.Write("Whick backend API Server or Connect Direct? [a d]: ");
            string input = Console.ReadLine();
            if (input == "a")
            {
                Console.Write("Enter the server: ");
                string host = Console.ReadLine();
                Console.Write("Enter the accesstoken: ");
                string accesstoken = Console.ReadLine();
                lovense = LovenseController.WithApiBackend(new Dictionary<string, string>() { ["host"] = host, ["accesstoken"] = accesstoken });
            }
            else if(input == "d")
            {
                Console.Write("Enter the host you want to control: ");
                lovense = LovenseController.WithConnectBackend(new Dictionary<string, string>() { ["host"] = Console.ReadLine() });
            }
            else
            {
                Console.WriteLine("Invalid option");
                return;
            }

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
                    Console.WriteLine(strength);
                    Command cmd = new CommandBuilder(toy).WithStrength(strength).Build();
                    lovense.SendCommand(cmd);
                    lastStrength = strength;
                    Thread.Sleep(400); //If you use the token api. Don't want to overload it        
                }
            }

            controller.Shutdown();

        }
    }
}
