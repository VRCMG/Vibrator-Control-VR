using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense.Toys
{
    public enum ToyTypes
    {
        Hush, Osci, Nora, Max2, Max, Lush, Domi, Edge
    }

    public class Toy
    {
        public Toy(string id, ToyTypes type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; set; }
        public ToyTypes Type { get; set; }

        public static ToyTypes getToyTypeByName(string name)
        {
            switch (name)
            {
                case "hush":
                    return ToyTypes.Hush;
                case "edge":
                    return ToyTypes.Edge;
                default:
                    return ToyTypes.Hush;
            }
        }
    }
}
