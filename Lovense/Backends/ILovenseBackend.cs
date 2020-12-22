using Lovense.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense.Backends
{
    public interface ILovenseBackend
    {
        void Setup(Dictionary<string, string> parameter);

        List<Toy> GetToys();

        void SendCommand(Command command);
    }
}
