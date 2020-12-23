using Lovense.Toys;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense.Backends
{
    //Inspired and Code used by https://github.com/sextech/lovense
    public class ConnectBackend : ILovenseBackend
    {
        private string host;
        private string port;
        private string connect;

        private List<Toy> toys;

        public List<Toy> GetToys()
        {
            return toys;
        }

        public void SendCommand(Command command)
        {
            switch (command.action)
            {
                case LovenseAction.Vibrate:
                    PerformGETRequest($"{connect}/Vibrate?v={command.strength}&t={command.toy.Id}");
                    break;
                case LovenseAction.Vibrate1:
                    PerformGETRequest($"{connect}/Vibrate1?v={command.strength}&t={command.toy.Id}");
                    break;
                case LovenseAction.Vibrate2:
                    PerformGETRequest($"{connect}/Vibrate2?v={command.strength}&t={command.toy.Id}");
                    break;
                case LovenseAction.Rotate:
                    string method = "Rotate" + (command.rotate == Rotate.Clockwise ? "Clockwise" : "") +(command.rotate == Rotate.AntiClockwise ? "AntiClockwise" : "");
                    PerformGETRequest($"{connect}/{method }?v={command.strength}&t={command.toy.Id}");
                    break;
                case LovenseAction.RotateChange:
                    PerformGETRequest($"{connect}/RotateChange?t={command.toy.Id}");
                    break;
                case LovenseAction.AirAuto:
                    PerformGETRequest($"{connect}/AirAuto?v={command.strength}&t={command.toy.Id}");
                    break;
                case LovenseAction.AirIn:
                    PerformGETRequest($"{connect}/AirIn?t={command.toy.Id}");
                    break;
                case LovenseAction.AirOut:
                    PerformGETRequest($"{connect}/AirOut?t={command.toy.Id}");
                    break;
                default:
                    Console.WriteLine($"Action: {command.action} is not supported");
                    break;
            }
        }

        public void Setup(Dictionary<string, string> parameter)
        {
            //TODO Autodiscover via https://api.lovense.com/api/lan/getToys

            if (!parameter.TryGetValue("host", out host))
                host = "localhost";
            if (!parameter.TryGetValue("port", out port))
                port = "34567";

            connect = $"http://{host}:{port}";

            IRestResponse response = PerformGETRequest($"{connect}/GetToys");

            toys = new List<Toy>();

            JObject json = JObject.Parse(response.Content);

            JObject toysJson = (JObject)json["data"];
            foreach (var toyKV in toysJson)
            {
                //Untested
                JObject toy = (JObject)toyKV.Value;
                if (toy["name"].ToString() == "Hush")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Hush));
                if (toy["name"].ToString() == "Edge")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Osci));
                if (toy["name"].ToString() == "Max")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Max));
                if (toy["name"].ToString() == "Max2")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Max2));
                if (toy["name"].ToString() == "Lush")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Lush));
                if (toy["name"].ToString() == "Nora")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Nora));
                if (toy["name"].ToString() == "Domi")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Domi));
                if (toy["name"].ToString() == "Osci")
                    toys.Add(new Toy(toyKV.Key, ToyTypes.Osci));
            }
        }

        private IRestResponse PerformGETRequest(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response;
        }
    }
}
