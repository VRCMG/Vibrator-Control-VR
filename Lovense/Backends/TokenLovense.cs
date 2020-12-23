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
    public class TokenLovense : ILovenseBackend
    {
        private CookieContainer _cookieJar = new CookieContainer();
        public string shortToken;
        public string longToken;
        public List<Toy> toys = new List<Toy>();

        private IRestResponse PerformGETRequest(string url)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = false,
                CookieContainer = _cookieJar
            };
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
        }

        private void GetToken()
        {
            IRestResponse response = PerformGETRequest($"https://c.lovense.com/c/{shortToken}");

            string url = (string)response.Headers.Single(x => x.Name == "Location").Value;
            Match match = Regex.Match(url, "play\\/(.+)");
            longToken = match.Groups[1].Value;
        }

        private void LoadToys()
        {
            IRestResponse response = PerformGETRequest($"https://c.lovense.com/app/ws/loading/{longToken}");
            JObject json = JObject.Parse(response.Content);

            JObject toys = (JObject)json["data"]["toyData"];
            foreach (var toy in toys)
            {
                toys.Add(new Toy(toy.Key, ToyTypes.Hush));
            }
        }

        private string GetEncodedCommand(Command cmd)
        {
            JObject toyaction = new JObject();
            switch (cmd.action)
            {
                case LovenseAction.Vibrate:
                    toyaction.Add("v", cmd.strength);
                    toyaction.Add("p", -1);
                    toyaction.Add("r", -1);
                    break;
                default:

                    Console.WriteLine("Not Implemented");
                    break;
            }

            JObject jsonCmd = new JObject();


            jsonCmd.Add("cate", "id");
            jsonCmd.Add("id", new JObject(new JProperty(cmd.toy.Id, toyaction)));

            return jsonCmd.ToString();
            //return "{\"cate\":\"id\",\"id\":{\"e733918a9831\":{\"v\":0,\"p\":-1,\"r\":-1}}}";
        }


        public void Setup(Dictionary<string, string> parameter)
        {
            this.shortToken = parameter["token"];
            GetToken();
            //Needed if token is temporary a secred coockie will be set on the second request
            PerformGETRequest($"https://c.lovense.com/app/ws/play/{longToken}");
            PerformGETRequest($"https://c.lovense.com/app/ws2/play/{longToken}");
            LoadToys();
        }


        public List<Toy> GetToys()
        {
            return toys;
        }

        public void SendCommand(Command command)
        {
            var client = new RestClient($"https://c.lovense.com/app/ws/command/{longToken}");
            client.FollowRedirects = false;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;

            request.AddParameter("order", GetEncodedCommand(command));
            IRestResponse response = client.Execute(request);
        }

    }
}
