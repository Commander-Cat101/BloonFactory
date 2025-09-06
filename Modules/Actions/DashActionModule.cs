using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;

namespace BloonFactory.Modules.Actions
{
    internal class DashActionModule : Module
    {
        public override string Name => "Dash";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Speed", 2.5f, 0, float.MaxValue));
            AddProperty(new IntModuleProperty("Distance", 25, int.MinValue, int.MinValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SetPositionActionModel("SetPositionModel", Id.ToString(), GetValue<int>("Distance"), GetValue<float>("Speed")));


        }
    }
}
