using BloonFactory.Modules.Display;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class DisplayCategory : Category
    {
        public override string Name => "Display";

        public override Type[] Modules => [ typeof(SimpleDisplayModule) ];
    }
}
