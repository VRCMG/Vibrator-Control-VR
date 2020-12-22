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
        Command command = new Command();

        public Command Build()
        {
            return command;
        }

        public CommandBuilder ForToy(Toy toy)
        {
            command.toy = toy;

            if (toy is Hush) command.action = Action.Vibrate;

            return this;
        }

        public CommandBuilder WithStrength(int strength)
        {
            //Only for vib
            if (strength > 20) strength = 20;
            if (strength < 0) strength = 0;
            command.strength = strength;

            return this;
        }
    }
}
