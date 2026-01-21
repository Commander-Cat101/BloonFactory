using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Spawning
{
    internal class ReplaceBloonModule : Module
    {
        public override string Name => "Replace Bloon";

        public override void GetLinkNodes()
        {
            AddInput<RoundModel>("Round");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new BloonEnumModuleProperty("Bloon", "BloonId", ((BloonTemplate)Template).TemplateId));
        }
        public override void ProcessModule()
        {
            RoundModel round = GetInputValue<RoundModel>("Round");
            string bloonId = ((BloonTemplate)Template).TemplateId;

            foreach (var group in round.groups.Where(a => a.bloon == GetValue<string>("BloonId")))
            {
                group.bloon = bloonId;
            }
        }
    }
}
