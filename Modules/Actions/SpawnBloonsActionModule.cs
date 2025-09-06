using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;

namespace BloonFactory.Modules.Actions
{
    internal class SpawnBloonsActionModule : Module
    {
        public override string Name => "Spawn Bloons";
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Bloon", Game.instance.model.bloons.Select(a => a.name).ToArray(), 0));
            AddProperty(new IntModuleProperty("Count", 10, 0, int.MaxValue));
            AddProperty(new FloatModuleProperty("Distance Ahead", 45, float.MinValue, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SpawnBloonsActionModel("SpawnBloonsActionModel", Id.ToString(), Game.instance.model.bloons[GetValue<int>("Bloon")].id, GetValue<int>("Count"), 0.02f
                , GetValue<float>("Distance Ahead"), 0, 0, new Il2CppStringArray(["BloonariusAttackSpew"]), new Il2CppStringArray(["BloonariusAttackSpewMoab"]), 1.5f, false, "Bloonarius"));
            
        }
    }
}
