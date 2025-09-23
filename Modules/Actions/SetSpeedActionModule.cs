using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Behaviors.Props;
using Il2CppAssets.Scripts.Data.Cosmetics.Props;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class SetSpeedActionModule : Module
    {
        public const string BehaviorName = "BloonFactory-SetSpeedActionModule";
        public override string Name => "Set Speed";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Multiplier", 2, 0, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SetSpeedPercentActionModel(BehaviorName, Id.ToString(), GetValue<float>("Multiplier"), false, 0, 0));
        }
    }
}
