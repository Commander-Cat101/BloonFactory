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
    internal class DrainLivesActionModule : Module
    {
        public override string Name => "Drain Lives";

        public override void GetModuleProperties()
        {
            AddProperty(new IntModuleProperty("Lives", 25, 0, int.MaxValue));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new DrainLivesActionModel("DrainLivesActionModel", Id.ToString(), GetValue<int>("Lives"), new PrefabReference("16977201d6852c348a8f90c77293f0d4 "), 2));
        }
    }
}
