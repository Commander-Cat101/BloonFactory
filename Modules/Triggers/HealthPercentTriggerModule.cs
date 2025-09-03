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
    internal class HealthPercentTriggerModule : TriggerModule
    {
        public override string Name => "Health Percent Trigger";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Interval", 5, 0, float.MaxValue));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());
            currentModel.AddBehavior(new HealthPercentTriggerModel("HealthPercentTriggerModel"));

            GetOutputsModules("Trigger").ProcessAll();
        }
    }
}
