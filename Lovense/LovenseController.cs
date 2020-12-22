using Lovense;
using Lovense.Backends;
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

namespace Lovense
{
    public class LovenseController
    {

        private ILovenseBackend backend;

        private LovenseController(ILovenseBackend backend, Dictionary<string, string> settings)
        {
            this.backend = backend;
            backend.Setup(settings);
        }

        public static LovenseController WithBackend(ILovenseBackend backend, Dictionary<string, string> settings)
        {
            return new LovenseController(backend, settings);
        }

        public static LovenseController WithTokenBackend(Dictionary<string, string> settings)
        {
            return new LovenseController(new TokenLovense(), settings);
        }

        public void SendCommand(Command cmd)
        {
            if (!backend.GetToys().Contains(cmd.toy))
            {
                throw new Exception("Toy not registered with this backend");
            }
            backend.SendCommand(cmd);
        }

        public List<Toy> GetToys()
        {
            return backend.GetToys();
        }



    }
}
