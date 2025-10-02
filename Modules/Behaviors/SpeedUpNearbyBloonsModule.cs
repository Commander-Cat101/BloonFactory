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
        public override string Name => "Speedup Behind Bloons";
        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Speed Multiplier", 2f, 0.1f, 99));
            AddProperty(new IntModuleProperty("Distance", 50, 0, int.MaxValue));
        }

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }

        public override void ProcessModule()
        {
            GetInputValue<BloonModel>("Bloon").AddBehavior(new BuffBloonSpeedModel("BuffBloonSpeedModel", GetValue<float>("Speed Multiplier"), GetValue<int>("Distance"), "VortexBloonSpeedBuff"));
        }
    }
}
