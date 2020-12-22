using Lovense;
using Lovense.Backends;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lovense
{
    public class Lovense
    {

        private ILovenseBackend backend;

        private Lovense(ILovenseBackend backend, Dictionary<string, string> settings)
        {
            this.backend = backend;
            backend.Setup(settings);
        }

        public static Lovense WithBackend(ILovenseBackend backend, Dictionary<string, string> settings)
        {
            return new Lovense(backend, settings);
        }

        public void SendCommand(Command cmd)
        {
            if (!backend.GetToys().Contains(cmd.toy))
            {
                throw new Exception("Toy not registered with this backend");
            }
            backend.SendCommand(cmd);
        }



    }
}
