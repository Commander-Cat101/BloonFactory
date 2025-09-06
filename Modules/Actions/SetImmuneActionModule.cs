using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Modules.Actions
{
    internal class SetImmuneActionModule : Module
    {
        public override string Name => "Set Immune";
        public override void GetModuleProperties()
        {
            AddProperty(new BoolModuleProperty("Set Immune", true));
        }
        public override void GetLinkNodes()
        {
            AddInput<Trigger>("Trigger");
        }

        public override void ProcessModule()
        {
            var trigger = GetInputValue<Trigger>("Trigger");
            trigger.bloonModel.AddBehavior(new SetImmuneActionModel("SetImmuneActionModel", Id.ToString(), GetValue<bool>("Set Immune"), new AudioClipReference("")));
        }
    }
}
