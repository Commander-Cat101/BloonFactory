using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Linq;

namespace BloonFactory.Modules.Actions
{
    internal class SpawnBloonsActionModule : Module
    {
        public override string Name => "Spawn Bloons";
        public override void GetModuleProperties()
        {
            AddProperty(new BloonEnumModuleProperty("Bloon", "BloonId"));
            AddProperty(new IntModuleProperty("Count", 10, 0, int.MaxValue));
            AddProperty(new FloatModuleProperty("Distance Ahead", 45, float.MinValue, float.MaxValue));

        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            try
            {
                var trigger = GetInputValue<Trigger>("Trigger");
                string id = GetValue<string>("BloonId");

                if (!Game.instance.model.bloons.Any(a => a.id == id))
                    return;

                trigger.bloonModel.AddBehavior(new SpawnBloonsActionModel("SpawnBloonsActionModel", Id.ToString(), id, GetValue<int>("Count"), 0.02f
                    , GetValue<float>("Distance Ahead"), 0, 0, new Il2CppStringArray(["BloonariusAttackSpew"]), new Il2CppStringArray(["BloonariusAttackSpewMoab"]), 1.5f, false, "Bloonarius"));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to add action. {ex}");
            }
            
        }
    }
}
