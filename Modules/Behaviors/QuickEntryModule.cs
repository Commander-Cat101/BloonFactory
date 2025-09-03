using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Behaviors
{
    internal class QuickEntryModule : Module
    {
        public override string Name => "Quick Entry";

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Speed Multiplier", 1f, 0, float.MaxValue));
            AddProperty(new IntSliderModuleProperty("Percent Of Track", 25, 0, 100));
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");
            bloon.AddBehavior(new QuickEntryModel("QuickEntryModel", GetValue<float>("Speed Multiplier"), GetValue<int>("Percent Of Track") / 100));
        }
    }
}
