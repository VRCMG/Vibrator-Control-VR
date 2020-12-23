using Lovense.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense
{
    public enum LovenseAction
    {
        Vibrate, Vibrate1, Vibrate2, Rotate, RotateChange, AirAuto, AirIn, AirOut
    }
    public enum Rotate
    {
        Normal, Clockwise, AntiClockwise
    }

    public struct Command
    {
        public Toy toy;
        public int strength;
        public LovenseAction action;
        public Rotate rotate;
    }

    public class CommandBuilder
    {
        Command command = new Command();

        public CommandBuilder(Toy toy)
        {
            command.toy = toy;
        }

        public Command Build()
        {
            return command;
        }

        
        public CommandBuilder WithAction(LovenseAction action)
        {
           command.action = action;

            return this;
        }

        public CommandBuilder WithRotationDirection(Rotate rotate)
        {
            command.rotate = rotate;

            return this;
        }

        public CommandBuilder WithStrength(int strength)
        {
            int maxstrength = command.action==LovenseAction.AirAuto ? 3:20;

            if (strength > maxstrength) strength = maxstrength;
            if (strength < 0) strength = 0;
            command.strength = strength;

            return this;
        }
    }
}
