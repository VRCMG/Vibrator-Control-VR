using Lovense.Toys;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lovense.Backends
{
    public class ApiLovense : ILovenseBackend
    {
        public List<Toy> toys = new List<Toy>();
        string host;
        string accesstoken;

        public void Setup(Dictionary<string, string> parameter)
        {
            Console.WriteLine("This backend is depricateded. Use the WS backend instead");
            host = parameter["host"];
            accesstoken = parameter["accesstoken"];

            toys.Add(new Toy(accesstoken, ToyTypes.Hush));

        }


        public List<Toy> GetToys()
        {
            return toys;
        }

        public void SendCommand(Command command)
        {
            var client = new RestClient($"https://{host}/sendCommand");

            var request = new RestRequest(Method.POST);
            if (command.action == LovenseAction.Vibrate)
                request.AddJsonBody(new { accesscode = accesstoken, action = "Vibrate", value = $"{command.strength}" });
            else if (command.action == LovenseAction.Vibrate1)
                request.AddJsonBody(new { accesscode = accesstoken, action = "Vibrate1", value = $"{command.strength}" });
            else if (command.action == LovenseAction.Vibrate2)
                request.AddJsonBody(new { accesscode = accesstoken, action = "Vibrate2", value = $"{command.strength}" });
            else
                return; //Unsuported

            
            client.Execute(request);
        }

    }
}
