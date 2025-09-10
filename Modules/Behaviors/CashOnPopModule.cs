using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Behaviors
{
    internal class CashOnPopModule : Module
    {
        public override string Name => "Cash On Pop";

        public override void GetLinkNodes()
        {
            AddInput<BloonModel>("Bloon");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Cash", 5, int.MinValue, int.MaxValue));
        }
        public override void ProcessModule()
        {
            var bloon = GetInputValue<BloonModel>("Bloon");

            if (bloon.HasBehavior<DistributeCashModel>())
                bloon.RemoveBehavior<DistributeCashModel>();

            bloon.AddBehavior(new DistributeCashModel("CashOnDropModel", GetValue<int>("Cash")));
        }
    }
}
