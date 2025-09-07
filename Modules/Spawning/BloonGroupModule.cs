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
    internal class BloonGroupModule : Module
    {
        public override string Name => "Bloon Group";

        public override void GetLinkNodes()
        {
            AddInput<RoundModel>("Round");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Count", 10, 1, int.MaxValue));
            AddProperty(new FloatModuleProperty("Start Time", 0, 0, float.MaxValue));
            AddProperty(new FloatModuleProperty("End Time", 60, 0, float.MaxValue));
        }
        public override void ProcessModule()
        {
            RoundModel round = GetInputValue<RoundModel>("Round");
            string bloonId = ((BloonTemplate)Template).TemplateId;
            round.AddBloonGroup(bloonId, GetValue<int>("Count"), GetValue<float>("Start Time") * 100, GetValue<float>("End Time") * 100);
        }
    }
}
