using Lovense.Toys;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Lovense.Backends
{
    public class ApiWSBackend : ILovenseBackend
    {
        public List<Toy> toys = new List<Toy>();
        string host;
        string accesstoken;
        private WebSocket websocket;

        public List<Toy> GetToys()
        {
            return toys;
        }

        public void SendCommand(Command command)
        {
            if (command.action == LovenseAction.Vibrate)
                
            websocket.Send(JsonConvert.SerializeObject(new { type= "send-command", uid = accesstoken, action = "Vibrate", value = $"{command.strength}" }));
            else if (command.action == LovenseAction.Vibrate1)
                websocket.Send(JsonConvert.SerializeObject(new { type = "send-command", uid = accesstoken, action = "Vibrate1", value = $"{command.strength}" }));
            else if (command.action == LovenseAction.Vibrate2)
                websocket.Send(JsonConvert.SerializeObject(new { type = "send-command", uid = accesstoken, action = "Vibrate2", value = $"{command.strength}" }));
        }

        public void Setup(Dictionary<string, string> parameter)
        {
            host = parameter["host"];
            if (!parameter.ContainsKey("host-mode"))
                accesstoken = parameter["accesstoken"];

            toys.Add(new Toy(accesstoken, ToyTypes.Hush));
            websocket = new WebSocket($"wss://{host}");
            
            if(!parameter.ContainsKey("host-mode"))
            websocket.OnMessage += (sender, e) => {
                JObject obj = JObject.Parse(e.Data);
                Console.WriteLine($"[RECV] {obj["message"]}");
            };

            websocket.Connect();
        }

        private Dictionary<Toy, string> toysToHosts = new Dictionary<Toy, string>();
        private Dictionary<string, Toy> uidToToys = new Dictionary<string, Toy>();
        private Dictionary<string, Toy> idToToys = new Dictionary<string, Toy>();

        public void SetupAsToyProvider(Dictionary<string, string> parameter)
        {
            parameter.Add("host-mode", "");
            Setup(parameter);
            var client = new RestClient("https://api.lovense.com/api/lan/getToys");

            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            JObject resultobj = JObject.Parse(response.Content);

            foreach (KeyValuePair<string, JToken> hosts in resultobj)
            {
                JObject hosts2 = hosts.Value as JObject;
                string host = $"{hosts2.Value<string>("domain")}:{hosts2.Value<string>("httpPort")}";

                foreach (KeyValuePair<string, JToken> toys in hosts2.Value<JObject>("toys"))
                {
                    JObject toys2 = toys.Value as JObject;
                    string id = toys2.Value<string>("id");
                    string name = toys2.Value<string>("name");
                    Toy toy = new Toy(id, Toy.getToyTypeByName(name));
                    toysToHosts.Add(toy, host);
                    idToToys.Add(id, toy);

                    websocket.Send(JsonConvert.SerializeObject(new { type = "register-toy", toy = id, custom = id }));
                }


            }
            

            websocket.OnMessage += (sender, e) =>
            {
                Recieve(e);
            };

        }

        private void Recieve(MessageEventArgs e)
        {
            JObject obj = JObject.Parse(e.Data);
            string type = obj.Value<string>("type");
            switch (type)
            {
                case "register-toy-result":
                    string accesscode = obj.Value<string>("uid");
                    string custom = obj.Value<string>("custom");
                    Toy toy = idToToys[custom];
                    uidToToys.Add(accesscode, toy);
                    Console.WriteLine($"Toy registered: accesscode: {accesscode} host: {toysToHosts[toy]} type: {toy.Type}");
                    break;
                case "command":
                    string id = obj.Value<string>("toy");
                    string action = obj.Value<string>("command");
                    string value = obj.Value<string>("value");
                    Toy destinationToy = idToToys[id];
                    string host = toysToHosts[destinationToy];

                    var client = new RestClient($"http://{host}/{action}?v={value}&t={destinationToy.Id}");
                    var request = new RestRequest(Method.GET);
                    client.Execute(request);

                    break;
                default:
                    break;
            }
        }
    }
}
