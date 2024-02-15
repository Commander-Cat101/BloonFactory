using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.API.Bloons
{
    public class CustomBloonSerializable
    {
        public string Name { get; set; } = string.Empty;

        public bool Camo = false;

        public bool Fortified = false;

        public bool Regrow = false;

        public float RegrowRate = 0;

        public int Health { get; set; } = 1;

        public float Speed { get; set; } = 25;

        public float CashDrop { get; set; } = 1;

        public float Size { get; set; } = 1;

        public float R { get; set; } = 255;
        public float G { get; set; } = 255;
        public float B { get; set; } = 255;

    }
}
