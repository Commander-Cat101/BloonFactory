using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace BloonFactory.Modules.Actions
{
    internal class RedirectBloonSpawnActionModule : Module
    {
        public override string Name => "Redirect Bloon Spawn";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Duration", 5, 0, float.PositiveInfinity));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new RedirectBloonSpawnActionModel("RedirectBloonSpawnActionModel", GetValue<float>("Duration"), 9999, new PrefabReference("ff187be8bcc6d834084e17ae5084f9b9"), Id.ToString(), false, new EffectModel("thing", new PrefabReference("e3e1a9b2926ffeb4cad93d3f74b42b62"), 1, 1)));

        }
    }
}
