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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class DestroyNearbyProjectilesActionModule : Module
    {
        public override string Name => "Destroy Nearby Projectiles";

        public override void GetModuleProperties()
        {
            AddProperty(new IntSliderModuleProperty("Heal Percent", 25, 0, 100));
            AddProperty(new IntModuleProperty("Heal Additive", 0, int.MinValue, int.MinValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new HealBloonActionModel("HealBloonAction", GetValue<int>("Heal Percent") / 100, GetValue<int>("Heal Additive"), Id.ToString()));
            
        }
    }
}
