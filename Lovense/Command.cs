using Lovense.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense
{
    public enum Action
    {
        Vibrate, Rotate, TODO
    }

    public struct Command
    {
        public Toy toy;
        public int strength;
        public Action action;
    }

    public class CommandBuilder
    {

    }
}
