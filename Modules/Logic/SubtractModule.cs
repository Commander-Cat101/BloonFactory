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
    internal class SubtractModule : Module
    {
        public override string Name => "Subtract";

        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Subtract Number", 1, int.MinValue, int.MaxValue));
            AddProperty(new SpaceModuleProperty(100));
        }
        public override void GetLinkNodes()
        {
            AddInput<float>("Number");
            AddOutput<float>("Number", GetValue);
        }
        public object GetValue()
        {
            return 1;
        }
    }
}
