using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class BuffNearbyBloonsActionModule : Module
    {
        public override string Name => "Buff Nearby Bloons Action";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Distance", 25, 0, float.PositiveInfinity));
            AddProperty(new FloatModuleProperty("Speed Buff", 2f, 0.1f, 99));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new BuffBloonsInRadiusActionModel("SellTowersInRadiusModel", Id.ToString(), GetValue<float>("Distance"), GetValue<float>("Speed Buff")));
        }
    }
}
