using BloonFactory.Modules.Logic;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class LogicCategory : Category
    {
        public override string Name => "Logic";

        public override Type[] Modules => [ typeof(CompareModule), typeof(SubtractModule) ];
    }
}
