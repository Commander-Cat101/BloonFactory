using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Tags
{
    internal class RegrowTagModule : Module
    {
        public override string Name => "Regrow Tag";

        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(100));
            AddProperty(new FloatModuleProperty("Regrow Rate", 3, 0.05f, 10));
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");
            bloon.SetRegrow(bloon.id, GetValue<float>("Regrow Rate"));
        }
    }
}
