using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Triggers
{
    internal class DamagedTriggerModule : TriggerModule
    {
        public override string Name => "Damaged Trigger";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Cooldown", 5, 0, float.MaxValue));
            AddProperty(new FloatSliderModuleProperty("Chance", 0.5f, 0, 1, 0.01f));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());
            currentModel.AddBehavior(new OnDamagedTriggerModel("OnDamagedTriggerModel", guids, GetValue<float>("Cooldown"), GetValue<float>("Chance")));

            GetOutputsModules("Trigger").ProcessAll();
        }
    }
}
