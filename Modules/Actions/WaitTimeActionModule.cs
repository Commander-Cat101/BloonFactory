using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class WaitTimeActionModule : Module
    {
        public override string Name => "Wait Time";
        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Time", 3, 0, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
            AddOutput<Trigger>("Trigger", () => GetInputValue<Trigger>("Trigger").With(this));
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            var guids = new Il2CppStringArray(GetOutputsModules("Trigger").AsGuids());
            trigger.bloonModel.AddBehavior(new WaitForSecondsActionModel("WaitForSecondsActionModel", GetValue<float>("Time"), Id.ToString(), guids));
            GetOutputsModules("Trigger").Where(a => !trigger.TriggeredModules.Contains(a)).ProcessAll();
        }
    }
}
