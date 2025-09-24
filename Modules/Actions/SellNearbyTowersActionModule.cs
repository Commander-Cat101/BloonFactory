using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class SellNearbyTowersActionModule : Module
    {
        public override string Name => "Sell Nearby Towers";

        public override void GetModuleProperties()
        {
            AddProperty(new FloatModuleProperty("Distance", 25, 0, float.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SellTowersInRadiusActionModel("SellTowersInRadiusModel", Id.ToString(), GetValue<float>("Distance"), new PrefabReference(""), 0));
            
        }
    }
}
