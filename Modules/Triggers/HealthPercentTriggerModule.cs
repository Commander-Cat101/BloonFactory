using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace BloonFactory.Modules.Triggers
{
    internal class HealthPercentTriggerModule : TriggerModule
    {
        public override string Name => "Health Percent Trigger";

        public override void GetModuleProperties()
        {
            AddProperty(new IntSliderModuleProperty("Percentage", 50, 0, 100));
        }

        public override void GetLinkNodes()
        {
            AddOutput<Trigger>("Trigger", () => new Trigger(currentModel));
        }

        public override void ProcessModule()
        {
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());

            float value = (float)GetValue<int>("Percentage") / 100;
            currentModel.AddBehavior(new HealthPercentTriggerModel("HealthPercentTriggerModel", false, new float[] { value }, guids, true));

            GetOutputsModules("Trigger").ProcessAll();
        }
    }
}
