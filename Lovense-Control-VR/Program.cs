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
            new Program().Run();

        }
        LovenseController lovense = null;
        Controller controller = new Controller();
        Toy toy;
        private void Run()
        {
            SetupBackend();

            controller.Setup();

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                Update();
            }

            controller.Shutdown();
        }
        private int lastStrength1 = 0;
        private int lastStrength2 = 0;
        private int mode = 0; //0 = normal, 1 = edge
        private void Update()
        {
            controller.RefreshValues();
            
            if (controller.modePressed)
            {
                mode = (mode + 1) % 2;
                if (mode == 0) controller.DisplayOverlay("Normal.png", 10000);
                else if (mode == 1) controller.DisplayOverlay("Edge.png", 10000);
            }
            if (mode == 0)
            {
                if (controller.level1 != lastStrength1)
                {
                    Console.WriteLine("Sending command");
                    Command cmd = new CommandBuilder(toy).WithAction(LovenseAction.Vibrate).WithStrength(controller.level1).Build();
                    lovense.SendCommand(cmd);
                    lastStrength1 = controller.level1;
                }
            }
            else if(mode == 1)
            {
                if (controller.level1 != lastStrength1 || controller.level2 != lastStrength2)
                {
                    Console.WriteLine("Sending command");
                    Command cmd = new CommandBuilder(toy).WithAction(LovenseAction.Vibrate1).WithStrength(controller.level1).Build();
                    Command cmd2 = new CommandBuilder(toy).WithAction(LovenseAction.Vibrate2).WithStrength(controller.level2).Build();
                    lovense.SendCommand(cmd);
                    lovense.SendCommand(cmd2);
                    lastStrength1 = controller.level1;
                    lastStrength2 = controller.level2;
                }
            }
            
        }

        private void SetupBackend()
        {
            Console.Write("Which backend API Server or Connect Direct? [a d]: ");
            string input = Console.ReadLine();
            if (input == "a")
            {
                Console.Write("Enter the server: ");
                string host = Console.ReadLine();
                Console.Write("Enter the accesstoken: ");
                string accesstoken = Console.ReadLine();
                lovense = LovenseController.WithApiBackend(new Dictionary<string, string>() { ["host"] = host, ["accesstoken"] = accesstoken });
            }
            else if (input == "d")
            {
                Console.Write("Enter the host you want to control: ");
                lovense = LovenseController.WithConnectBackend(new Dictionary<string, string>() { ["host"] = Console.ReadLine() });
            }
            else
            {
                Console.WriteLine("Invalid option");
                return;
            }
            toy = lovense.GetToys()[0];
        }

    }
}
