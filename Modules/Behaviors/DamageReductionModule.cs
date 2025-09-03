using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Behaviors
{
    internal class DamageReductionModule : Module
    {
        public override string Name => "Damage Reduction";

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new IntSliderModuleProperty("Percentage", 25, 0, 100));
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");
            bloon.AddBehavior(new DamageReductionModel("DamageReduction", GetValue<int>("Percentage") / 100));
            bloon.AddBehavior(new Il2CppAssets.Scripts.Models.Bloons.Behaviors.QuickEntryModel("QuickEntryModel", 5, 0.3f));
        }
    }
}
