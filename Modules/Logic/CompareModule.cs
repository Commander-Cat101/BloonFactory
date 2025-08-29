using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Api.Components;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using FactoryCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Logic
{
    internal class CompareModule : Module
    {
        public override string Name => "Compare";

        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Compare Type", ["Equal", "Greater than", "Less than"], 0));
            AddProperty(new IntModuleProperty("Compare Number", 1, int.MinValue, int.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<float>("Number");
            AddOutput<Trigger>("Trigger", () => null);
            AddInput<Trigger>("Trigger");
        }
    }
}
