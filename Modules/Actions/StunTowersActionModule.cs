using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace BloonFactory.Modules.Actions
{
    internal class StunTowersActionModule : Module
    {
        public override string Name => "Stun Towers";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Distance", 25, float.MinValue, float.MinValue));
            AddProperty(new FloatModuleProperty("Duration", 3, float.MinValue, float.MinValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new StunTowersInRadiusActionModel("StunTowers", Id.ToString(), GetValue<float>("Distance"), GetValue<float>("Duration"), 0.3f, new PrefabReference(""), false));
        }
    }
}
