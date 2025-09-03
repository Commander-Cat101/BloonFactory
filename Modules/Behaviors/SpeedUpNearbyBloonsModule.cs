using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Behaviors
{
    internal class SpeedUpNearbyBloonsModule : Module
    {
        public override string Name => "Speedup Nearby Bloons";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Speed Buff", 2f, 0.1f, 99));
            AddProperty(new IntModuleProperty("Radius", 50, 0, int.MaxValue));
        }

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }

        public override void ProcessModule()
        {
            GetInputValue<BloonModel>("Bloon").AddBehavior(new BuffBloonSpeedModel("BuffBloonSpeedModel", GetValue<float>("Speed Buff"), GetValue<int>("Radius"), "VortexBloonSpeedBuff"));
        }
    }
}
