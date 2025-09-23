using BTD_Mod_Helper.Api.Enums;
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
    internal class CamoTagModule : Module
    {
        public override string Name => "Camo Tag";

        public override void GetModuleProperties()
        {
            AddProperty(new SpaceModuleProperty(100));
        }
        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void ProcessModule()
        {
            //GetInputValue<BloonModel>("Bloon").SetCamo(true);

            var bloon = GetInputValue<BloonModel>("Bloon");
            bloon.isCamo = true;
            bloon.AddTag(BloonTag.Camo);
        }
    }
}
